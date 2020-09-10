Imports cnrl.exploitation.common.ORMapping
Imports System.ComponentModel
Imports System.Data.OracleClient

Public Class KeyValueLookup

    Protected _dbconfig As IDBConfig

    Public Sub New(dbconfig As IDBConfig)
        _dbconfig = dbconfig
    End Sub

    Protected Function GetDBConfig() As IDBConfig
        Return _dbconfig
    End Function

    Private Function ConcatFilters(filters As Object()) As String
        If filters.Count = 0 Then
            Return Nothing
        ElseIf filters.Count = 1 Then
            Return filters(0)
        Else
            Dim allfilter As String = filters(0)
            For i As Integer = 1 To filters.Length - 1
                allfilter &= GetDBConfig.LookupFilterSeprator & filters(i)
            Next

            Return allfilter
        End If
    End Function

    Public Function GetSpecialLookupList(lookupFunction As String, lookupName As String, Optional lookupFilter As Object = Nothing) As BindingList(Of KeyValueEntity)
        Return GetSpecialLookupList(lookupFunction, lookupName, {lookupFilter})
    End Function

    'This funciton provide a way that user can specify lookup function name other than the one in dbconfig
    Public Function GetSpecialLookupList(lookupFunction As String, lookupName As String, filters As Object()) As BindingList(Of KeyValueEntity)
        Return DoGetLookupList(lookupFunction, lookupName, filters)
    End Function

    Public Function GetLookupList(lookupName As String, filters As Object()) As BindingList(Of KeyValueEntity)
        Return DoGetLookupList(GetDBConfig.LookupFunction, lookupName, filters)
    End Function

    Protected Function DoGetLookupList(lookupFunction As String, lookupName As String, filters As Object()) As BindingList(Of KeyValueEntity)
        Dim kvLoader As New KeyValueLoader
        Dim list As BindingList(Of KeyValueEntity)
        Dim dbutil = New DBUtil(GetDBConfig)

        Dim command As OracleCommand = New OracleCommand(GetDBConfig.DatabaseSchema + "." + lookupFunction)
        command.CommandType = System.Data.CommandType.StoredProcedure

        ' Add Parameters
        command.Parameters.Add(New System.Data.OracleClient.OracleParameter("in_lookup_name", OracleClient.OracleType.VarChar))
        command.Parameters.Add(New System.Data.OracleClient.OracleParameter("in_lookup_filter", OracleClient.OracleType.VarChar))
        command.Parameters.Add(New System.Data.OracleClient.OracleParameter("in_lookup_filter_count", OracleClient.OracleType.Number))
        command.Parameters.Add(New System.Data.OracleClient.OracleParameter("cur_out", System.Data.OracleClient.OracleType.Cursor, 0, System.Data.ParameterDirection.Output, True, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

        Dim lookupFilter As String = Nothing
        Dim filterCnt As Integer = 0

        If filters IsNot Nothing AndAlso Not (filters.Count = 1 And filters(0) Is Nothing) Then
            lookupFilter = ConcatFilters(filters)
            filterCnt = filters.Count
        End If

        dbutil.SetParameterValue(command, "in_lookup_name", lookupName)
        dbutil.SetParameterValue(command, "in_lookup_filter", lookupFilter)
        dbutil.SetParameterValue(command, "in_lookup_filter_count", filterCnt)

        list = kvLoader.LoadEntities(GetDBConfig, command)

        Return list
    End Function

    Public Function GetLookupList(lookupName As String, Optional lookupFilter As Object = Nothing) As BindingList(Of KeyValueEntity)
        Return GetLookupList(lookupName, {lookupFilter})
    End Function

    Public Function GetFirstLookupEntity(lookup_name As String, filters As Object()) As KeyValueEntity
        Dim entityList As BindingList(Of KeyValueEntity) = GetLookupList(lookup_name, filters)

        If entityList.Count > 0 Then
            Return entityList(0)
        Else
            'Return a blank one 
            Return New KeyValueEntity()
        End If
    End Function

    Public Function GetFirstLookupEntity(lookup_name As String, Optional lookupFilter As Object = Nothing) As KeyValueEntity
        Return GetFirstLookupEntity(lookup_name, {lookupFilter})
    End Function

    Public Function GetLookupValue(key As Object, lookupName As String, filters As Object()) As Object
        Dim list As BindingList(Of KeyValueEntity) = GetLookupList(lookupName, filters)

        For Each entity In list
            If Object.Equals(entity.Key, key) Then
                Return entity.Value
            End If
        Next

        Return Nothing
    End Function

    Public Function GetLookupValue(key As Object, lookupName As String, Optional lookupFilter As Object = Nothing) As Object
        Return GetLookupValue(key, lookupName, {lookupFilter})
    End Function

    Public Shared Function GetYNBooleanList() As BindingList(Of KeyValueEntity)
        Dim list As New BindingList(Of KeyValueEntity)
        list.Add(New KeyValueEntity(True, "Y"))
        list.Add(New KeyValueEntity(False, "N"))

        Return list
    End Function
End Class
