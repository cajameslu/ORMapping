Imports System.Data
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data.OracleClient

''' <summary>
''' The base class for all BusinessDataAccess classes.
''' It provides common functions such as load entity/entities, execute command.
''' </summary>
''' <remarks></remarks>
Public MustInherit Class BusinessDataAcess
    Implements IBusinessDataAccess

    Private _businessEntity As IBusinessEntity

    Public Sub New(businessEntity As IBusinessEntity)
        _businessEntity = businessEntity
    End Sub

    Public Function CreateConnection() As IDbConnection Implements IBusinessDataAccess.CreateConnection
        Return GetDBConfig.CreateConnection()
    End Function

    Public MustOverride Function GetDBConfig() As IDBConfig Implements IBusinessDataAccess.GetDBConfig

    Public MustOverride Function CreateEntitiesList() As IBindingList Implements IBusinessDataAccess.CreateEntityList

    Public Overridable Function CreateSelectByPkCommand() As System.Data.IDbCommand Implements IBusinessDataAccess.CreateSelectByPkCommand
        Return Nothing
    End Function

    Public Overridable Function CreateSelectByIdCommand() As System.Data.IDbCommand Implements IBusinessDataAccess.CreateSelectByIdCommand
        Return Nothing
    End Function

    Public Overridable Function CreateSelectCommand() As System.Data.IDbCommand Implements IBusinessDataAccess.CreateSelectCommand
        Return Nothing
    End Function

    Public Overridable Function CreateInsertCommand() As System.Data.IDbCommand Implements IBusinessDataAccess.CreateInsertCommand
        Return Nothing
    End Function

    Public Overridable Function CreateUpdateCommand() As System.Data.IDbCommand Implements IBusinessDataAccess.CreateUpdateCommand
        Return Nothing
    End Function

    Public Overridable Function CreateDeleteCommand() As System.Data.IDbCommand Implements IBusinessDataAccess.CreateDeleteCommand
        Return Nothing
    End Function

    Public MustOverride Function CreateEntity() As IBusinessEntity Implements IBusinessDataAccess.CreateEntity

    Public Overridable Function InsertCommandOutParamToPKMap() As String(,) Implements IBusinessDataAccess.InsertCommandOutParamToPKMap
        Return Nothing
    End Function

    Protected Overridable Function CreateCommand(ByVal storedProcName As String, Optional cmdType As CommandType = CommandType.StoredProcedure) As IDbCommand
        Return New DBUtil(GetDBConfig).CreateCommand(storedProcName, cmdType)
    End Function

    Protected Friend Sub UpdateEntity(transMan As TransactionManager) Implements IBusinessDataAccess.UpdateEntity
        ExecuteCommand(CreateUpdateCommand(), transMan)
    End Sub

    Protected Friend Sub InsertEntity(transMan As TransactionManager) Implements IBusinessDataAccess.InsertEntity
        ExecuteCommand(CreateInsertCommand(), transMan, True)
    End Sub

    Protected Friend Sub DeleteEntity(transMan As TransactionManager) Implements IBusinessDataAccess.DeleteEntity
        ExecuteCommand(CreateDeleteCommand(), transMan)
    End Sub

    Public ReadOnly Property BusinessEntity() As IBusinessEntity
        Get
            Return _businessEntity
        End Get
    End Property

    Protected Sub ExecuteCommand(command As IDbCommand, transMan As TransactionManager, Optional retrievePK As Boolean = False)
        command.Connection = transMan.Connection
        command.Transaction = transMan.Transaction
        command.ExecuteNonQuery()

        If retrievePK Then
            RetrievePKFromInsert(command)
        End If
    End Sub

    Private Sub RetrievePKFromInsert(command As IDbCommand)
        Dim pkMap As String(,) = InsertCommandOutParamToPKMap()
        If pkMap IsNot Nothing Then
            Dim columnFieldMap As Dictionary(Of String, FieldInfo) = _businessEntity.GetColmnNameToFieldInfoMap()

            For i As Integer = 0 To pkMap.GetUpperBound(0)
                Dim pkOutputParam As String = pkMap(i, 0)
                Dim pkColName As String = pkMap(i, 1).ToUpper()

                If command.Parameters.Contains(pkOutputParam) Then
                    If columnFieldMap.ContainsKey(pkColName) Then
                        _businessEntity.SetFieldInternal(columnFieldMap(pkColName), CType(command.Parameters(pkOutputParam), OracleParameter).Value, True)
                    End If
                End If
            Next
        End If
    End Sub

    Class ColumnIndexToFieldInfoRecord
        Public columnIndex As Integer
        Public fieldInfo As FieldInfo

        Public Sub New(aColumnIndex As Integer, aFieldInfo As FieldInfo)
            Me.columnIndex = aColumnIndex
            Me.fieldInfo = aFieldInfo
        End Sub
    End Class

    Private Function GetColumnIndexToFieldInfoMap(reader As IDataReader) As List(Of ColumnIndexToFieldInfoRecord)
        Dim mappedColumnFields As New List(Of ColumnIndexToFieldInfoRecord)

        Dim columnNameFieldInfoMap As Dictionary(Of String, FieldInfo) = _businessEntity.GetColmnNameToFieldInfoMap()

        For colIndex As Integer = 0 To reader.FieldCount - 1
            Dim columnName = reader.GetName(colIndex)
            If columnNameFieldInfoMap.ContainsKey(columnName) Then
                mappedColumnFields.Add(New ColumnIndexToFieldInfoRecord(colIndex, columnNameFieldInfoMap(columnName)))
            End If
        Next

        Return mappedColumnFields
    End Function

    Friend Function LoadSingleEntity(command As IDbCommand, entity As IBusinessEntity, Optional throwNoEntityFoundException As Boolean = False, Optional throwMultiEntityFoundException As Boolean = False) As Boolean Implements IBusinessDataAccess.LoadSingleEntity
        'Dim timer As DebugTimer = New DebugTimer
        Dim connection As IDbConnection = CreateConnection()
        Dim reader As IDataReader = Nothing

        Try
            command.Connection = connection
            reader = command.ExecuteReader()

            entity.OnDataLoading()

            If reader.Read Then

                Dim map As List(Of ColumnIndexToFieldInfoRecord) = GetColumnIndexToFieldInfoMap(reader)
                'Dim timer2 As DebugTimer = New DebugTimer
                For Each rec In map
                    entity.SetFieldInternal(rec.fieldInfo, reader(rec.columnIndex), True)
                Next
                'timer2.Print("BusinessDataAcess.LoadSingleEntity.read (" & entity.GetType.Name() & ")")

                If throwMultiEntityFoundException Then
                    If reader.Read Then
                        Throw New MultiEntityFoundException(entity.GetType.Name)
                    End If
                End If

                'Dim timer3 As DebugTimer = New DebugTimer
                entity.OnDataLoaded()
                'timer3.Print("BusinessDataAcess.LoadSingleEntity.entity.OnDataLoaded (" & entity.GetType.Name() & ")")

                Return True
            Else

                If throwNoEntityFoundException Then
                    Throw New NoEntityFoundException(entity.GetType.Name)
                End If

                Return False
            End If

        Finally
            If reader IsNot Nothing Then
                Try
                    reader.Close()
                Catch

                End Try
            End If


            DBUtil.CloseConnection(connection)
        End Try

        'timer.Print("BusinessDataAcess.LoadSingleEntity(" & entity.GetType.Name() & ")")

        Return False
    End Function

    Function LoadEntities(sql As String, Optional entityList As IBindingList = Nothing) As IBindingList Implements IBusinessDataAccess.LoadEntities
        Return LoadEntities(New DBUtil(GetDBConfig).CreateCommand(sql), entityList)
    End Function

    Friend Function LoadEntities(command As IDbCommand, Optional entityList As IBindingList = Nothing) As IBindingList Implements IBusinessDataAccess.LoadEntities
        If entityList Is Nothing Then
            entityList = CreateEntitiesList()
        Else
            entityList.Clear()
        End If

        Dim connection As IDbConnection = Nothing
        Dim entity As IBusinessEntity = Nothing
        Dim reader As IDataReader = Nothing

        Try
            If command.Connection Is Nothing Then
                connection = CreateConnection()
                command.Connection = connection
            End If

            reader = command.ExecuteReader()
            Dim map As List(Of ColumnIndexToFieldInfoRecord) = GetColumnIndexToFieldInfoMap(reader)

            While reader.Read
                entity = CreateEntity()
                entity.OnDataLoading()
                For Each rec In map
                    entity.SetFieldInternal(rec.fieldInfo, reader(rec.columnIndex), True, False, False)
                Next
                entity.OnDataLoaded()
                entityList.Add(entity)
            End While

        Catch e As Exception
            Throw e
        Finally
            If reader IsNot Nothing Then
                Try
                    reader.Close()
                Catch

                End Try
            End If

            If connection IsNot Nothing Then
                DBUtil.CloseConnection(connection)
            End If
        End Try

        Return entityList
    End Function

    Protected Sub SetParameterValue(ByVal command As IDbCommand, ByVal parameterName As String, ByVal value As Object)
        DBUtil.SetParameterValue(command, parameterName, value)
    End Sub

    Protected Overridable Function GetField(fieldName As String) As Object
        Return _businessEntity.GetField(fieldName)
    End Function

    Public Function CreateParameter(paramName As String, paramType As OracleType, Optional direction As ParameterDirection = ParameterDirection.Input) As OracleParameter
        Return DBUtil.CreateParameter(paramName, paramType, direction)
    End Function

    Public Sub AddParameter(ByVal command As IDbCommand, ByVal parameterName As String, ByVal parameterValue As Object, paramType As OracleType, Optional direction As ParameterDirection = ParameterDirection.Input)
        command.Parameters.Add(CreateParameter(parameterName, paramType, direction))
        SetParameterValue(command, parameterName, parameterValue)
    End Sub
End Class
