Imports System.ComponentModel

Public MustInherit Class VirtualEntity
    Implements INotifyPropertyChanged, IDataErrorInfo, IDataEntity

    Protected _compositeEntities As New List(Of IBusinessEntity)

    Protected _parentEntity As IBusinessEntity
    Protected _isReadonly As Boolean

    Protected _isNavigatingAway As Boolean

    Private Shared _typeMap As ITypeMap = TypeMap.GetTypeMap()

    Public MustOverride Function GetVirutalEntityInfo() As VirtualEntityInfo

    Public Sub AddEntity(entity As IBusinessEntity)
        If entity IsNot Nothing Then
            For Each ent In _compositeEntities
                If ent.GetType.Equals(entity.GetType) Then
                    Throw New Exception("Entity of this type already exists: " & entity.GetType.Name)
                End If
            Next

            If Not GetVirutalEntityInfo.AllowType(entity.GetType) Then
                Throw New Exception("Entity of this type not allowed in " & Me.GetType().Name & " : " & entity.GetType.Name)
            End If

            _compositeEntities.Add(entity)
            AddHandler entity.PropertyChanged, AddressOf PropertyChangedHandler
        End If
    End Sub

    'Normalize values stored in the map for consistence
    Protected Overridable Function NormalizeValue(aType As Type, fieldValue As Object) As Object
        Dim targetValue As Object = ValueConverter.ConvertNull(aType, fieldValue)

        targetValue = ValueConverter.Convert(targetValue, aType)

        Return targetValue
    End Function

    Public Function GetPropertyValue(propertyName As String) As Object
        Dim internalEntity As IBusinessEntity = GetInternalEntity(propertyName)
        Dim internalProperty As String = GetInternalFieldName(propertyName)

        If internalEntity IsNot Nothing Then
            Dim propertyInfo As Reflection.PropertyInfo = internalEntity.GetType.GetProperty(internalProperty)
            Return propertyInfo.GetValue(internalEntity, Nothing)
        Else
            Return Nothing
        End If
    End Function

	Public Sub SetPropertyValue(propertyName As String, propertyValue As Object)
		Dim internalEntity As IBusinessEntity = GetInternalEntity(propertyName)
		Dim internalProperty As String = GetInternalFieldName(propertyName)

		If internalEntity IsNot Nothing Then
			Dim propertyInfo As Reflection.PropertyInfo = internalEntity.GetType.GetProperty(internalProperty)
			Dim normalizedValue As Object = NormalizeValue(propertyInfo.PropertyType, propertyValue)
			propertyInfo.SetValue(internalEntity, normalizedValue, Nothing)
		End If
	End Sub

	Protected Sub SetField(fieldName As String, fieldValue As Object, Optional checkBusinessRule As Boolean = False) Implements IDataEntity.SetField
		SetPropertyValue(fieldName, fieldValue)
	End Sub

    Public Property IsNavigatingAway() As Boolean Implements IEntity.IsNavigatingAway
        Get
            Return _isNavigatingAway
        End Get
        Set(value As Boolean)
            _isNavigatingAway = value
        End Set
    End Property

    Public ReadOnly Property [Error] As String Implements System.ComponentModel.IDataErrorInfo.Error
		Get
			Dim err As String = ""
			For Each ent In _compositeEntities
				Dim cErr As String = ent.Error
				If cErr IsNot Nothing AndAlso cErr.Length > 0 Then
					err &= vbNewLine & cErr
				End If
			Next

			Return err
		End Get
	End Property

	Default Public ReadOnly Property Item(columnName As String) As String Implements System.ComponentModel.IDataErrorInfo.Item
		Get
			Dim entity As IBusinessEntity = GetInternalEntity(columnName)
			Dim propertyName As String = GetInternalFieldName(columnName)

			Return entity(propertyName)
		End Get
	End Property

	Public Event PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

	Public Sub PropertyChangedHandler(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs)
		Dim propertyName As String = e.PropertyName
		Dim fullPropertyName = GetVirutalEntityInfo.GetAliasByEntityType(sender.GetType) & "." & propertyName

		RaisePropertyChange(fullPropertyName)

		If GetVirutalEntityInfo.IsUniqueProperty(propertyName) Then
			RaisePropertyChange(propertyName)
		End If
	End Sub

	Public Sub RaisePropertyChange(propName As String)
		RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propName))

		Dim dependentProperties As HashSet(Of String) = _typeMap.GetDependentProperties(Me.GetType, propName)
		If dependentProperties IsNot Nothing Then
			For Each depProp In dependentProperties
				RaisePropertyChange(depProp)
			Next
		End If
	End Sub

	Private Function GetInternalFieldName(fieldName As String) As String
		If fieldName.Contains(".") Then
			Return fieldName.Split(".")(1)
		Else
			Return fieldName
		End If
	End Function

	Private Function GetInternalEntity(fieldName As String) As IBusinessEntity
		If fieldName.Contains(".") Then
			Dim typeAlias As String = fieldName.Split(".")(0)
			Dim entType As Type = GetVirutalEntityInfo.GetEntityTypeByAlias(typeAlias)

			If entType IsNot Nothing Then
				Return GetInternalEntityByType(entType)
			Else
				Throw New Exception("No Entity type named: " & typeAlias)
			End If
		Else
			Dim props As List(Of VirtualProperty) = GetVirutalEntityInfo.GetPropertyListByName(fieldName)
			If props IsNot Nothing Then
				If props.Count = 1 Then
					Return GetInternalEntityByType(props(0).EntityType.EntityType)
				Else
					Throw New Exception("Multiple entity type has filed : " & fieldName)
				End If
			Else
				Throw New Exception("No Entity type has field name: " & fieldName)
			End If
		End If

		Return Nothing
	End Function

	Public Function GetInternalEntityByType(aType As Type) As IBusinessEntity
		For Each ent In _compositeEntities
			If ent.GetType.Equals(aType) Then
				Return ent
			End If
		Next

		Return Nothing
	End Function

	Public Function GetDBConfig() As IDBConfig Implements IEntity.GetDBConfig
		For Each ent In _compositeEntities
			Dim dbconfig As IDBConfig = ent.GetDBConfig
			If dbconfig IsNot Nothing Then
				Return dbconfig
			End If
		Next

		Return Nothing
	End Function

	Public Overridable Sub Refresh() Implements IEntity.Refresh
		For Each ent In _compositeEntities
			ent.Refresh()
		Next
	End Sub

	Public Overridable Sub RefreshAll() Implements IEntity.RefreshAll
		For Each ent In _compositeEntities
			ent.RefreshAll()
		Next
	End Sub

	Public Overridable Sub Save(Optional checkBusinessRules As Boolean = True, Optional cascading As Boolean = True) Implements IEntity.Save
		For Each ent In _compositeEntities
			If ent.IsDirty Then
				ent.Save(checkBusinessRules, cascading)
			End If
		Next
	End Sub

	Public Overridable Sub SaveInternal(transMan As TransactionManager, cascading As Boolean) Implements IEntity.SaveInternal
		For Each ent In _compositeEntities
			ent.SaveInternal(transMan, cascading)
		Next
	End Sub

	Public Overridable Sub Delete(cascadeDelete As Boolean) Implements IEntity.Delete
		'for safety, leave this as blank
		'subclasses may overrides this to provide delete action
	End Sub

	Protected Friend Overridable Sub DeleteInternal(transMan As TransactionManager, cascadeDelete As Boolean) Implements IEntity.DeleteInternal
		'for safety, leave this as blank
		'subclasses may overrides this to provide delete action
	End Sub

	Protected Friend Overridable Sub ResetDeletedFlag() Implements IEntity.ResetDeletedFlag
		'for safety, leave this as blank
		'subclasses may overrides this to provide delete action
	End Sub

	Public Overridable ReadOnly Property IsDirty() As Boolean Implements IEntity.IsDirty
		Get
			For Each ent In _compositeEntities
				If ent.IsDirty Then
					Return True
				End If
			Next

			Return False
		End Get
	End Property

    Public Function CheckBusinessRules(ByRef errMsg As String, Optional cascadeCheck As Boolean = True, Optional force As Boolean = False) As Boolean Implements IEntity.CheckBusinessRules
        Dim ret As Boolean = CheckBusinessRules(cascadeCheck, force)

        errMsg = GetBusinessRuleError()
        Return ret
    End Function

    Public Function CheckBusinessRules(Optional cascadeCheck As Boolean = True, Optional force As Boolean = False) As Boolean Implements IEntity.CheckBusinessRules
        Return CheckBusinessRulesInternal(cascadeCheck, force)
    End Function

    Protected Friend Overridable Function CheckBusinessRulesInternal(cascadeCheck As Boolean, Optional force As Boolean = False) As Boolean Implements IEntity.CheckBusinessRulesInternal
        Dim ret As Boolean = True

        For Each ent In _compositeEntities
            ret = ret And ent.CheckBusinessRulesInternal(cascadeCheck, force)
        Next

        Return ret
    End Function

    Public Function GetBusinessRuleErrorMsgSet() As HashSet(Of String) Implements IEntity.GetBusinessRuleErrorMsgSet
        Dim errMsgSet As New HashSet(Of String)

        For Each ent In _compositeEntities
            errMsgSet.UnionWith(ent.GetBusinessRuleErrorMsgSet)
        Next

        Return errMsgSet
    End Function

	Public Overridable Function GetBusinessRuleError() As String Implements IEntity.GetBusinessRuleError
		Dim errMsg As String = ""
        For Each curMsg As String In GetBusinessRuleErrorMsgSet()
            errMsg &= vbNewLine & curMsg
        Next

        Return errMsg
	End Function

	Public Overridable Sub ClearBusinessRules(Optional cascading As Boolean = True) Implements IEntity.ClearBusinessRules
		For Each ent In _compositeEntities
			ent.ClearBusinessRules(cascading)
		Next
	End Sub

	Public Property ParentEntity() As IBusinessEntity Implements IEntity.ParentEntity
		Get
			Return _parentEntity
		End Get
		Set(value As IBusinessEntity)
			_parentEntity = value
		End Set
	End Property

	Public Overridable Function GetAncestorEntity(type As Type) As IBusinessEntity Implements IEntity.GetAncestorEntity
		Dim parent As IBusinessEntity = ParentEntity

		While parent IsNot Nothing
			If parent.GetType = type Then
				Return parent
			Else
				parent = parent.ParentEntity
			End If
		End While

		Return Nothing
	End Function

	'Whole entity is read only?
	Public Overridable Property IsReadOnly() As Boolean Implements IEntity.IsReadOnly
		Get
			Return _isReadonly
		End Get
		Set(value As Boolean)
			_isReadonly = value

			For Each ent In _compositeEntities
				ent.IsReadOnly = value
			Next
		End Set
	End Property

	Public Overridable Sub Clear() Implements IDataEntity.Clear
		For Each ent As IDataEntity In _compositeEntities
			ent.Clear()
		Next
	End Sub

	Public Function BusinessRuleErrorList() As IEnumerable(Of FieldMessage) Implements IDataEntity.BusinessRuleErrorList
		Dim errList As New List(Of FieldMessage)

		For Each ent In _compositeEntities
			Dim curList As IEnumerable(Of FieldMessage) = ent.BusinessRuleErrorList
			If curList IsNot Nothing Then
				For Each fieldMsg In curList
					errList.Add(New FieldMessage(GetVirutalEntityInfo.GetPropetyFullName(ent.GetType, fieldMsg.FieldName), fieldMsg.MessageList))
				Next
			End If
		Next

		Return errList
	End Function

	Public Function BulkSetFields(fieldNames As IEnumerable(Of String), fieldValues As IEnumerable(Of Object), Optional continueOnError As Boolean = True) As IEnumerable(Of FieldMessage) Implements IDataEntity.BulkSetFields
		Dim errorFields As FieldMessageMap = Nothing

		For i As Integer = 0 To Math.Min(fieldNames.Count, fieldValues.Count) - 1
			Try
				SetPropertyValue(fieldNames(i), fieldValues(i))
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


	Public Sub RestoreToBackupFieldsAllRecursive() Implements IDataEntity.RestoreToOrignialFieldsAllRecursive
		For Each ent In _compositeEntities
			ent.RestoreToOrignialFieldsAllRecursive()
		Next
    End Sub

    Public Overridable Function GetEntityIdString() As String Implements IDataEntity.GetEntityIdString
        Return ""
    End Function
End Class
