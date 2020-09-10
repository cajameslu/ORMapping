Imports System.ComponentModel

Public Class KeyValueLoader
    Private _keyColumn As Integer
    Private _valueColumn As Integer
    Private _descColumn As Integer
    Private _tagColumn As Integer

    Public Sub New(Optional keyCol As Integer = 0, Optional ValueCol As Integer = 1, Optional descCol As Integer = 2, Optional tagCol As Integer = 3)
        _keyColumn = keyCol
        _valueColumn = ValueCol
        _descColumn = descCol
        _tagColumn = tagCol
    End Sub

    Public Property KeyColumn() As Integer
        Get
            Return _keyColumn
        End Get
        Set(ByVal v As Integer)
            _keyColumn = v
        End Set
    End Property

    Public Property ValueColumn() As Integer
        Get
            Return _valueColumn
        End Get
        Set(ByVal v As Integer)
            _valueColumn = v
        End Set
    End Property

    Public Property DescColumn() As Integer
        Get
            Return _descColumn
        End Get
        Set(ByVal v As Integer)
            _descColumn = v
        End Set
    End Property

    Public Property TagColumn() As Integer
        Get
            Return _tagColumn
        End Get
        Set(ByVal v As Integer)
            _tagColumn = v
        End Set
    End Property

    Public Function LoadEntities(dbconfig As IDBConfig, query As String, queryType As System.Data.CommandType) As BindingList(Of KeyValueEntity)
        If queryType = CommandType.StoredProcedure Then
            query = dbconfig.DatabaseSchema + "." + query
        End If

        Dim command As System.Data.OracleClient.OracleCommand = New System.Data.OracleClient.OracleCommand(query)
        command.CommandType = queryType
        Return LoadEntities(dbconfig, command)
    End Function

    Public Function LoadEntities(dbconfig As IDBConfig, command As IDbCommand, Optional ByVal paramValues As Dictionary(Of String, Object) = Nothing) As BindingList(Of KeyValueEntity)
        Dim entities As IBindingList = New BindingList(Of KeyValueEntity)()
        Dim connection As IDbConnection = dbconfig.CreateConnection()

        Try
            If paramValues IsNot Nothing Then
                For Each p In paramValues.Keys
                    DBUtil.SetParameterValue(command, p, paramValues(p))
                Next
            End If

            command.Connection = connection
            Dim reader As IDataReader = command.ExecuteReader()

            While reader.Read
                Dim entity = New KeyValueEntity()

                entity.Key = reader(_keyColumn)
                entity.Value = reader(_valueColumn)

                If reader.FieldCount > DescColumn Then
                    entity.Desc = reader(_descColumn)
                End If

                If reader.FieldCount > TagColumn Then
                    entity.Tag = reader(_tagColumn)
                End If

                entities.Add(entity)
            End While

            reader.Close()
        Catch e As Exception
            Throw e
        Finally
            DBUtil.CloseConnection(connection)
        End Try

        Return entities
    End Function

    
End Class
