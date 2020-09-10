Imports System.Reflection
Imports System.ComponentModel
Imports System.Data.OracleClient
Imports cnrl.exploitation.common.ORMapping

''' <summary>
''' The base class of all Business Entity classes.
''' It provides all buiness entity operation funcitons. 
''' It maintains the status of the entity.
''' It stores all the data for the entity.
''' It is the CORE class for ORMapping library.
''' </summary>
''' <remarks></remarks>
Public MustInherit Class BusinessEntityCore
    Implements IBusinessEntity

    'Events
    'Public Event DatabaseDataLoaded(sender As IBusinessEntity) _
    '    Implements IBusinessEntity.DatabaseDataLoaded

    Public Event UiWritableChanged(sender As IBusinessEntity, propertyName As String) _
        Implements IBusinessEntity.UiWritableChanged

    Public Event PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs) _
        Implements INotifyPropertyChanged.PropertyChanged

    Private Shared _typeMap As ITypeMap = TypeMap.GetTypeMap()

    'Permissons
    Private _isReadOnly As Boolean = False

    Private _isNullEntity As Boolean = False

    'Fieds
    Private _fields As New EntityData(Me.GetFieldNameToFieldInfoMap)
    Private _originalFields As MyDictionary(Of String, Object)
    Private _backupFields As MyDictionary(Of String, Object)

    'Parents and children
    Private _parentEntity As IBusinessEntity
    Private _childEntities As New BusinessBindingList(Of IEntity)(Me)

    Private _businessRules As IBusinessRules

    Private _isCheckingRule As Boolean = False
    Private _isNavigatingAway As Boolean = False

    Public ReadOnly Property BusinessRules As IBusinessRules Implements IBusinessEntity.BusinessRules
        Get
            If _businessRules Is Nothing Then
                _businessRules = CreateBusinessRules()
            End If

            Return _businessRules
        End Get
    End Property

    Private ReadOnly Property OriginalFields As MyDictionary(Of String, Object)
        Get
            If _originalFields Is Nothing Then
                _originalFields = New MyDictionary(Of String, Object)()
            End If
            Return _originalFields
        End Get
    End Property

    Private ReadOnly Property BackupFields As MyDictionary(Of String, Object)
        Get
            If _backupFields Is Nothing Then
                _backupFields = New MyDictionary(Of String, Object)()
            End If
            Return _backupFields
        End Get
    End Property

    Protected MustOverride Function CreateDataAccess() As IBusinessDataAccess Implements IBusinessEntity.CreateDataAccess

    Protected MustOverride Function CreateBusinessRules() As IBusinessRules Implements IBusinessEntity.CreateBusinessRules

    Public ReadOnly Property IsNotNullEntity As Boolean
        Get
            Return Not _isNullEntity
        End Get
    End Property

    Public Property IsNullEntity As Boolean
        Get
            Return _isNullEntity
        End Get
        Set(value As Boolean)
            _isNullEntity = value
        End Set
    End Property

    Public Overridable Function GetEntityIdString() As String Implements IBusinessEntity.GetEntityIdString
        Return ""
    End Function

    Protected Overridable Function IsPropertyWritable(ByVal propertyName As String) As Boolean Implements IBusinessEntity.IsPropertyWritable
        Return Not IsReadOnly
    End Function

    'Usually subclass SHOULD NOT override this method, instead it should override CheckPropertyUiWritable
    Public Function IsPropertyUiWritable(ByVal propertyName As String) As Boolean Implements IBusinessEntity.IsPropertyUiWritable
        If Not IsPropertyWritable(propertyName) Then
            Return False
        End If

        Return CheckPropertyUiWritable(propertyName)
    End Function

    'Subclass should override this method to provide ui writable info
    Protected Overridable Function CheckPropertyUiWritable(ByVal propertyName As String) As Boolean
        Return True
    End Function

    'Return a list of properties that their UI Writable might change when named proerty changes
    Protected Overridable Function GetUiWritableDependentProperties(propertyName As String) As String()
        Return Nothing
    End Function

    Protected Overridable Sub RaiseUiWritableChanged(propertyName As String)
        RaiseEvent UiWritableChanged(Me, propertyName)
    End Sub

    Protected Overridable Sub RaiseAllUiWritableChanged()
        For Each prop In _typeMap.GetPropertyTypeMap(Me.GetType).Keys
            RaiseUiWritableChanged(prop)
        Next
    End Sub

    Public Overridable Property IsReadOnly() As Boolean Implements IBusinessEntity.IsReadOnly
        Get
            Return _isReadOnly
        End Get
        Set(value As Boolean)
            Dim oldValue As Boolean = _isReadOnly
            _isReadOnly = value

            If oldValue <> value Then
                RaiseAllUiWritableChanged()
            End If
        End Set
    End Property

    Public Property ParentEntity() As IBusinessEntity Implements IBusinessEntity.ParentEntity
        Get
            Return _parentEntity
        End Get
        Set(value As IBusinessEntity)
            _parentEntity = value
        End Set
    End Property

    Public Function GetAncestorEntity(type As Type) As IBusinessEntity Implements IBusinessEntity.GetAncestorEntity
        Dim parent As IEntity = ParentEntity

        While parent IsNot Nothing
            If parent.GetType = type Then
                Return parent
            Else
                parent = parent.ParentEntity
            End If
        End While

        Return Nothing
    End Function

    Protected Sub AddChildEntity(child As IEntity) Implements IBusinessEntity.AddChildEntity
        _childEntities.Add(child)
    End Sub

    Protected Sub ClearChildEntities() Implements IBusinessEntity.ClearChildEntities
        _childEntities.Clear()
    End Sub

    Public Overridable Sub SetDefaultValues() Implements IBusinessEntity.SetDefaultValues

    End Sub

    Protected Overridable Sub OnDataLoaded() Implements IBusinessEntity.OnDataLoaded
        SetDefaultValues()
        ResetState()

        'set this flag
        _isLastLoadSuccessful = True

        'RaiseEvent DatabaseDataLoaded(Me)
    End Sub

    Protected Overridable Sub OnDataLoading() Implements IBusinessEntity.OnDataLoading
        ClearOriginalFields()
        ClearBackupFields()
        ClearBusinessRules()

        'clear this flag
        _isLastLoadSuccessful = False
    End Sub

    Public Sub ClearBusinessRules(Optional cascading As Boolean = True) Implements IEntity.ClearBusinessRules
        If _businessRules IsNot Nothing Then
            _businessRules.ClearAll()
        End If

        If cascading Then
            _childEntities.ClearBusinessRules(cascading)
        End If
    End Sub

    Private Sub ClearOriginalFields()
        If _originalFields IsNot Nothing Then
            _originalFields.Clear()
        End If
    End Sub

    Protected Sub ClearFields() Implements IBusinessEntity.ClearFields
        Dim changedFieldNames As List(Of String) = _fields.Clear()

        For Each fieldName In changedFieldNames
            RaisePropertyChange(fieldName)
        Next
    End Sub

    'Sub class can overrides this method to clear customized (non - database) properties
    Public Overridable Sub Clear() Implements IDataEntity.Clear
        ClearFields()
        ClearOriginalFields()
        ClearBackupFields()
        ClearBusinessRules(False)
    End Sub

    Public Overridable Sub ClearAll() Implements IBusinessEntity.ClearAll
        Clear()
        UnLoadChildEntities()
    End Sub

    Protected Friend Function GetColmnNameToFieldInfoMap() As Dictionary(Of String, FieldInfo) Implements IBusinessEntity.GetColmnNameToFieldInfoMap
        Return _typeMap.GetColumnNameToFieldInfoMap(Me.GetType)
    End Function

    Protected Friend Function GetFieldNameToFieldInfoMap() As Dictionary(Of String, FieldInfo) Implements IBusinessEntity.GetFieldNameToFieldInfoMap
        Return _typeMap.GetFieldNameToFieldInfoMap(Me.GetType)
    End Function

    Protected Function GetFieldInfor(fieldName As String) As FieldInfo
        Return GetFieldNameToFieldInfoMap()(fieldName)
    End Function

    Public Function HasField(fieldName As String) As Boolean
        Return GetFieldNameToFieldInfoMap().ContainsKey(fieldName)
    End Function

    'Normalize values stored in the map for consistence
    Protected Overridable Function NormalizeValue(fieldName As String, fieldValue As Object) As Object Implements IBusinessEntity.NormalizeValue
        Dim fieldInfo As FieldInfo = GetFieldInfor(fieldName)
        Dim targetValue As Object = ValueConverter.ConvertNull(fieldInfo.FieldType, fieldValue)

        'If some field doing converting themselves, don't do type convert here
        If fieldInfo.FieldTypeCheck Then
            targetValue = ValueConverter.Convert(targetValue, fieldInfo.FieldType)
        End If

        Return targetValue
    End Function

    Protected Overridable Sub OnSetField(fieldName As String, newFieldValue As Object, oldFieldValue As Object, fromDbLoading As Boolean) Implements IBusinessEntity.OnSetField
        'Override in sub class as needed
    End Sub

    Protected Sub SetField(fieldName As String, fieldValue As Object, Optional checkBusinessRule As Boolean = False) Implements IBusinessEntity.SetField
        Dim fieldNameToFieldInfoMap = GetFieldNameToFieldInfoMap()

        If fieldNameToFieldInfoMap.ContainsKey(fieldName) Then
            Dim fieldInfo As FieldInfo = fieldNameToFieldInfoMap(fieldName)

            If IsPropertyWritable(fieldName) Then
                SetFieldInternal(fieldInfo, fieldValue, False, checkBusinessRule)
            Else
                'When entity property already bound to a combobox, if the combox do a reload of list items,
                'this code will be hit, which cause issue. This looks like a Microsoft Combobox issue, 
                'but don't know how to fix this, comment out this Exception throwing code for now 
                ' -- James Lu
                ' Throw New ModifyReadonlyFieldException(Me.GetType.Name, fieldName)
            End If
        Else
            Throw New NoFieldFoundException(Me.GetType.Name, fieldName)
        End If
    End Sub

    Protected Sub SetFieldInternal(fieldInfo As FieldInfo, fieldValue As Object, fromDbLoading As Boolean, Optional checkBusinessRule As Boolean = False, Optional raisePropertyChangeEvent As Boolean = True) Implements IBusinessEntity.SetFieldInternal
        'find previous value
        Dim previousValue As Object = _fields(fieldInfo.FieldIndex)
        fieldValue = NormalizeValue(fieldInfo.FieldName, fieldValue)

        'set new value
        _fields(fieldInfo.FieldIndex) = fieldValue

        Dim changed As Boolean = Not Object.Equals(previousValue, fieldValue)

        If changed Then
            'Check if value changed
            If Not fromDbLoading Then
                'if from db loading, no need to change original field as it will be clear out anyway
                If Not OriginalFields.ContainsKey(fieldInfo.FieldName) Then
                    'not in original map, add it
                    OriginalFields(fieldInfo.FieldName) = previousValue
                ElseIf Object.Equals(OriginalFields(fieldInfo.FieldName), fieldValue) Then
                    'Already in original map, but is same as current value, remove it
                    OriginalFields.Remove(fieldInfo.FieldName)
                End If

                If checkBusinessRule Then
                    Me.CheckBusinessRules()
                End If
            End If
        End If

        OnSetField(fieldInfo.FieldName, fieldValue, previousValue, fromDbLoading)

        If changed And raisePropertyChangeEvent Then
            RaisePropertyChange(fieldInfo.FieldName)
        End If
    End Sub

    Public Function BulkSetFields(fieldNames As IEnumerable(Of String), fieldValues As IEnumerable(Of Object), Optional continueOnError As Boolean = True) As IEnumerable(Of FieldMessage) Implements IBusinessEntity.BulkSetFields
        Dim errorFields As FieldMessageMap = Nothing

        For i As Integer = 0 To Math.Min(fieldNames.Count, fieldValues.Count) - 1
            Try
                SetField(fieldNames(i), fieldValues(i))
            Catch ex As Exception
                If errorFields Is Nothing Then
                    errorFields = New FieldMessageMap
                End If

                errorFields.AddFieldMessage(fieldNames(i), ex.ToString)

                If Not continueOnError Then
                    Exit For
                End If
            End Try
        Next

        If errorFields IsNot Nothing Then
            Return errorFields.FieldMessageList
        End If

        Return Nothing
    End Function

    Public Sub RaiseAllPropertyChange() Implements IBusinessEntity.RaiseAllPropertyChange
        For Each propName In GetFieldNameToFieldInfoMap().Keys
            RaisePropertyChange(propName)
        Next
    End Sub

    Public Sub RaisePropertyChange(propName As String) Implements IBusinessEntity.RaisePropertyChange
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propName))

        Dim dependentProperties As HashSet(Of String) = _typeMap.GetDependentProperties(Me.GetType, propName)
        If dependentProperties IsNot Nothing Then
            For Each depProp In dependentProperties
                RaisePropertyChange(depProp)
            Next
        End If

        Dim uiWritableProperties As String() = GetUiWritableDependentProperties(propName)
        If uiWritableProperties IsNot Nothing Then
            For Each uiWritableProp In uiWritableProperties
                RaiseUiWritableChanged(uiWritableProp)
            Next
        End If

        'If propName = "WellBhDesc" Then
        '    CheckBusinessRules(, True)
        'End If
    End Sub


    Public Sub RestoreToOriginlaField(fieldName As String) Implements IBusinessEntity.RestoreField
        If _originalFields IsNot Nothing Then
            If OriginalFields.ContainsKey(fieldName) Then
                SetField(fieldName, OriginalFields(fieldName))
            End If
        End If
    End Sub

    Public Sub RestoreToOriginlaFieldsAll() Implements IBusinessEntity.RestoreToOriginlaFieldsAll
        If _originalFields IsNot Nothing Then
            Dim orilFields As MyDictionary(Of String, Object) = New MyDictionary(Of String, Object)(_originalFields)

            For Each fieldName As String In orilFields.Keys
                SetField(fieldName, OriginalFields(fieldName))
            Next
        End If
    End Sub

    Public Sub RestoreToOrignialFieldsAllRecursive() Implements IEntity.RestoreToOrignialFieldsAllRecursive
        Me.RestoreToOriginlaFieldsAll()

        _childEntities.RestoreToOrignialFieldsAllRecursive()
    End Sub
    Public Function GetField(fieldName As String) As Object Implements IBusinessEntity.GetField
        Return _fields(fieldName)
    End Function

    Public Function GetOriginalField(fieldName As String) As Object Implements IBusinessEntity.GetOriginalField
        If _originalFields IsNot Nothing AndAlso _originalFields.ContainsKey(fieldName) Then
            Return _originalFields(fieldName)
        Else
            Return _fields(fieldName)
        End If
    End Function

    Public Function IsFieldChanged(fieldName As String) As Boolean Implements IBusinessEntity.IsFieldChanged
        Return _originalFields IsNot Nothing AndAlso _originalFields.ContainsKey(fieldName)
    End Function

    Public Overridable Function LoadEntities(sql As String, Optional entityList As IBindingList = Nothing) As IBindingList Implements IBusinessEntity.LoadEntities
        Return CreateDataAccess().LoadEntities(sql, entityList)
    End Function

    Public Overridable Function LoadEntities(command As System.Data.IDbCommand, Optional entityList As IBindingList = Nothing) As IBindingList Implements IBusinessEntity.LoadEntities
        Return CreateDataAccess().LoadEntities(command, entityList)
    End Function

    Public Overridable Function LoadEntities(Optional entityList As IBindingList = Nothing) As IBindingList Implements IBusinessEntity.LoadEntities
        Return LoadEntities(CreateDataAccess().CreateSelectCommand(), entityList)
    End Function

    Public Overridable Function LoadMeByPk(Optional throwNoEntityFoundException As Boolean = False, Optional throwMultiEntityFoundException As Boolean = False) As Boolean Implements IBusinessEntity.LoadMeByPk
        Dim dataAccess As IBusinessDataAccess = CreateDataAccess()
        Return dataAccess.LoadSingleEntity(dataAccess.CreateSelectByPkCommand(), Me, False, True)
    End Function

    Public Sub Refresh() Implements IEntity.Refresh
        LoadMeByPk()
    End Sub

    Public Overridable Sub RefreshAll() Implements IEntity.RefreshAll
        LoadMeByPk()
        LoadChildEntities()
    End Sub

    Public Overridable Function LoadEntitiesById(Optional entityList As IBindingList = Nothing) As IBindingList Implements IBusinessEntity.LoadEntitiesById
        Dim dataAccess As IBusinessDataAccess = CreateDataAccess()
        Return dataAccess.LoadEntities(dataAccess.CreateSelectByIdCommand(), entityList)
    End Function

    Public Overridable Function LoadMeById(Optional throwNoEntityFoundException As Boolean = False, Optional throwMultiEntityFoundException As Boolean = False) As Boolean Implements IBusinessEntity.LoadMeById
        Dim dataAccess As IBusinessDataAccess = CreateDataAccess()
        Return dataAccess.LoadSingleEntity(dataAccess.CreateSelectByIdCommand(), Me, False, False)
    End Function

    Public Overridable Sub LoadChildEntities() Implements IBusinessEntity.LoadChildEntities
        ClearChildEntities()

        'Override in subclass if it has child entities
        'Call AddChildEntity for every child entity to make Child Entity operations work
    End Sub

    Public Overridable Sub UnLoadChildEntities() Implements IBusinessEntity.UnLoadChildEntities
        ClearChildEntities()
    End Sub

    Public Sub Delete(cascadeDelete As Boolean) Implements IBusinessEntity.Delete
        Dim transMan As New TransactionManager(Me.CreateDataAccess.GetDBConfig)
        Try
            DeleteInternal(transMan, cascadeDelete)
            transMan.Commit()
        Catch e As Exception
            transMan.RollBack()
            ResetDeletedFlag()
            Throw e
        Finally
            transMan.CloseConnection()
        End Try
    End Sub

    Protected Sub ResetDeletedFlag() Implements IEntity.ResetDeletedFlag
        IsDeleted = False
        _childEntities.ResetDeletedFlag()
    End Sub

    Protected Friend Sub DeleteInternal(transMan As TransactionManager, cascadeDelete As Boolean) Implements IBusinessEntity.DeleteInternal
        If cascadeDelete Then
            _childEntities.DeleteInternal(transMan, cascadeDelete)
        End If

        IsDeleted = True
        SaveMe(transMan)
    End Sub

    Public Overridable Sub Save(Optional checkbusinessRules As Boolean = True, Optional cascading As Boolean = True) Implements IEntity.Save
        If IsDirty Then

            If checkbusinessRules Then
                Dim errMsg As String = Nothing
                If Not Me.CheckBusinessRules(errMsg, cascading) Then
                    Throw New Exception("Business Rules check failed: " & vbCrLf & errMsg)
                End If
            End If

            Dim transMan As TransactionManager = New TransactionManager(CreateDataAccess.GetDBConfig)
            Try
                SaveInternal(transMan, cascading)
                transMan.Commit()
            Catch ex As InvalidConstraintException
                transMan.RollBack()
            Catch e As Exception
                transMan.RollBack()
                Throw e
            Finally
                transMan.CloseConnection()
            End Try
        End If
    End Sub

    Protected Friend Overridable Sub SaveInternal(transMan As TransactionManager, cascading As Boolean) Implements IEntity.SaveInternal
        SaveMe(transMan)
        If cascading Then
            _childEntities.SaveInternal(transMan, cascading)
        End If
    End Sub

    'Save entity to database
    Protected Overridable Sub SaveMe(transMan As TransactionManager)
        If IsMeDirty Then
            If IsNew Then
                If Not IsDeleted Then
                    CreateDataAccess().InsertEntity(transMan)
                    ResetState()
                End If
            ElseIf IsDeleted Then
                If Not IsDeleteCommitted Then
                    CreateDataAccess().DeleteEntity(transMan)
                    _isDeleteCommitted = True
                    'not reset the IsDelete flag, otherwise user 
                    'don't know if it's a deleted one or normal one
                End If
            ElseIf IsModified Then
                CreateDataAccess().UpdateEntity(transMan)
                ResetState()
            End If
        End If
    End Sub

    Public ReadOnly Property IsDirty() As Boolean Implements IBusinessEntity.IsDirty
        Get
            Return IsMeDirty OrElse _childEntities.IsDirty
        End Get
    End Property

    Public Property IsNavigatingAway() As Boolean Implements IEntity.IsNavigatingAway
        Get
            Return _isNavigatingAway
        End Get
        Set(value As Boolean)
            _isNavigatingAway = value
        End Set
    End Property

    Public Function GetBusinessRuleErrorMsgSet() As HashSet(Of String) Implements IEntity.GetBusinessRuleErrorMsgSet
        Dim errMsgSet As New HashSet(Of String)

        If BusinessRules IsNot Nothing Then
            errMsgSet.UnionWith(BusinessRules.GetErrorSet)
        End If

        For Each ent In _childEntities
            errMsgSet.UnionWith(ent.GetBusinessRuleErrorMsgSet)
        Next

        Return errMsgSet
    End Function

    Public Function GetBusinessRuleError() As String Implements IEntity.GetBusinessRuleError
        Dim errMsg As String = ""

        For Each msg As String In GetBusinessRuleErrorMsgSet()
            errMsg &= vbNewLine & msg
        Next

        If errMsg <> "" Then
            errMsg = GetEntityIdString() & errMsg
        End If

        Return errMsg
    End Function

    Public Overridable Function CheckBusinessRules(ByRef errMsg As String, Optional cascadeCheck As Boolean = True, Optional force As Boolean = False) As Boolean Implements IBusinessEntity.CheckBusinessRules
        Dim ret As Boolean = CheckBusinessRules(cascadeCheck, force)
        errMsg = GetBusinessRuleError()
        Return ret
    End Function

    Public Function CheckBusinessRules(Optional cascadeCheck As Boolean = True, Optional force As Boolean = False) As Boolean Implements IBusinessEntity.CheckBusinessRules
        Return CheckBusinessRulesInternal(cascadeCheck, force)
    End Function

    Protected Function CheckBusinessRulesInternal(cascadeCheck As Boolean, Optional force As Boolean = False) As Boolean Implements IBusinessEntity.CheckBusinessRulesInternal
        Dim ret As Boolean = CheckMyBusinessRules(force)

        If cascadeCheck Then
            _childEntities.IsNavigatingAway = IsNavigatingAway
            ret = ret And _childEntities.CheckBusinessRulesInternal(cascadeCheck)
        End If

        Return ret
    End Function

    Public Overridable Function CheckMyBusinessRules(Optional force As Boolean = False) As Boolean Implements IBusinessEntity.CheckMyBusinessRules
        _isCheckingRule = True

        Dim ret As Boolean = True

        'Try

        ClearBusinessRules(False)

        If BusinessRules IsNot Nothing AndAlso (force Or MyBusinessRuleCheckNeeded) Then
            ret = BusinessRules.CheckBusinessRules()
        End If
        'Finally
        _isCheckingRule = False
        'End Try

        Return ret
    End Function

    Public ReadOnly Property IsCheckingRule As Boolean Implements IBusinessEntity.IsCheckingRule
        Get
            Return _isCheckingRule
        End Get
    End Property

    'Public Property IsNavigatingAway As Boolean Implements IBusinessEntity.IsNavigatingAway
    '    Get
    '        Return _isNavigatingAway
    '    End Get
    '    Set
    '        _isNavigatingAway = Value
    '    End Set
    'End Property

    Public Overridable ReadOnly Property MyBusinessRuleCheckNeeded As Boolean Implements IBusinessEntity.MyBusinessRuleCheckNeeded
        Get
            Return IsMeDirty
        End Get
    End Property

    Public Function GetDBConfig() As IDBConfig Implements IEntity.GetDBConfig
        Dim da As IBusinessDataAccess = Me.CreateDataAccess

        If da IsNot Nothing Then
            Return da.GetDBConfig
        Else
            For Each ent In _childEntities
                Dim dbconfig As IDBConfig = ent.GetDBConfig
                If dbconfig IsNot Nothing Then
                    Return dbconfig
                End If
            Next
        End If

        Return Nothing
    End Function

    Public Sub BackupAllFields()
        BackupFields.Clear()

        For Each fieldName In GetFieldNameToFieldInfoMap().Keys
            BackupFields.Add(fieldName, _fields(fieldName))
        Next
    End Sub

    Public Sub RestoreToBackupFields()
        If _backupFields IsNot Nothing Then
            For Each fieldName In _backupFields.Keys
                SetField(fieldName, _backupFields(fieldName))
            Next
        End If
    End Sub

    Public Sub BackupField(fieldName As String)
        BackupFields.Add(fieldName, _fields(fieldName))
    End Sub

    Public Sub RestoreToBackupField(fieldName As String)
        If _backupFields IsNot Nothing AndAlso _backupFields.ContainsKey(fieldName) Then
            SetField(fieldName, _backupFields(fieldName))
        End If
    End Sub

    Public Function GetBackupField(fieldName As String) As Object
        If _backupFields IsNot Nothing AndAlso _backupFields.ContainsKey(fieldName) Then
            Return _backupFields(fieldName)
        Else
            Return _fields(fieldName)
        End If
    End Function

    Public Sub ClearBackupFields()
        If _backupFields IsNot Nothing Then
            _backupFields.Clear()
        End If
    End Sub

    Public Function BusinessRuleErrorList() As IEnumerable(Of FieldMessage) Implements IBusinessEntity.BusinessRuleErrorList
        If BusinessRules IsNot Nothing Then
            Return BusinessRules.FieldErrorList
        Else
            Return Nothing
        End If
    End Function

    Public ReadOnly Property [Error] As String Implements IDataErrorInfo.Error
        Get
            If BusinessRules IsNot Nothing Then
                Return BusinessRules.GetErrors
            Else
                Return Nothing
            End If
        End Get
    End Property

    Default Public ReadOnly Property Item(columnName As String) As String Implements IDataErrorInfo.Item
        Get
            If BusinessRules IsNot Nothing Then
                Return BusinessRules.GetError(columnName)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public Sub AddError(fieldName As String, msg As String)
        BusinessRules.AddError(fieldName, msg)
    End Sub

    Public Sub RemoveError(fieldName As String)
        BusinessRules.RemoveError(fieldName)
    End Sub

    Private _isLastLoadSuccessful As Boolean = False

    Public ReadOnly Property IsLastLoadSuccessful() As Boolean
        Get
            Return _isLastLoadSuccessful
        End Get
    End Property


#Region "State management"
    'Status
    Private _isNew As Boolean = True
    Private _isDeleted As Boolean = False
    Private _isDeleteCommitted As Boolean = False

    'Flag indicates if this is an new entity (not in database yet)
    Public Property IsNew() As Boolean Implements IBusinessEntity.IsNew
        Get
            Return _isNew
        End Get
        Set(value As Boolean)
            _isNew = value
        End Set
    End Property

    'Flag indicates if an entity has been marked as deleted by user
    Public Property IsDeleted() As Boolean Implements IBusinessEntity.IsDeleted
        Get
            Return _isDeleted
        End Get
        Set(value As Boolean)
            _isDeleted = value
        End Set
    End Property

    'Flag indicates if delete has been committed
    Public ReadOnly Property IsDeleteCommitted() As Boolean Implements IBusinessEntity.IsDeleteCommitted
        Get
            Return _isDeleteCommitted
        End Get
    End Property

    'Flag indicates if any field has been modified
    Public ReadOnly Property IsModified() As Boolean Implements IBusinessEntity.IsModified
        Get
            Return _originalFields IsNot Nothing AndAlso _originalFields.Count > 0
        End Get
    End Property

    'Entity State list:
    '1. New Entity: (IsNew = true and IsDeleted = false), need to save to database : IsDirty = true
    '2. New but Deleted: (IsNew = true and IsDeleted = true), no need to save to database : IsDirty = false
    '3. Deleted Entity: (IsNew = false and IsDeleted = true and IsDeleteCommitted = false), need to save to database: IsDirty = true
    '4. Deleted and Committed: (IsNew = false and IsDeleted = true and IsDeleteCommitted = true), no need to save to database : IsDirty = false
    '5. Modifed Entity: (IsNew = false and IsDeleted = false and IsModifed = true), need to save to database: IsDirty = true
    '6. All others: no need to save to database: IsDirt = false
    Public Overridable ReadOnly Property IsMeDirty() As Boolean Implements IBusinessEntity.IsMeDirty
        Get
            Dim dirty As Boolean = False

            If IsNew Then
                If Not IsDeleted Then
                    'New and not deleted entity
                    dirty = True
                End If
            ElseIf IsDeleted Then
                If Not IsDeleteCommitted Then
                    'Deleted and not committed entity
                    dirty = True
                End If
            ElseIf IsModified Then
                'Modified entity
                dirty = True
            End If

            Return dirty
        End Get
    End Property

    Public ReadOnly Property IsDiscarded() As Boolean Implements IBusinessEntity.IsDiscarded
        Get
            If IsNew And IsDeleted Then
                Return True
            ElseIf IsDeleted And IsDeleteCommitted Then
                Return True
            End If

            Return False
        End Get
    End Property

    Protected Sub ResetState() Implements IBusinessEntity.ResetState
        _isDeleted = False
        _isNew = False
        _isDeleteCommitted = False

        _originalFields = Nothing
        _backupFields = Nothing
        _businessRules = Nothing
    End Sub

#End Region
End Class
