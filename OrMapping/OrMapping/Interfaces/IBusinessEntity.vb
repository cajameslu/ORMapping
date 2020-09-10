Imports System.ComponentModel
Imports System.Data.OracleClient

Public Interface IBusinessEntity
    Inherits IDataEntity, INotifyPropertyChanged, IDataErrorInfo

    Event UiWritableChanged(sender As IBusinessEntity, propertyName As String)
    'Event DatabaseDataLoaded(sender As IBusinessEntity)

    ReadOnly Property BusinessRules As IBusinessRules

    Function CreateDataAccess() As IBusinessDataAccess
    Function CreateBusinessRules() As IBusinessRules

    'Field operation
    Function GetColmnNameToFieldInfoMap() As Dictionary(Of String, FieldInfo)
    Function GetFieldNameToFieldInfoMap() As Dictionary(Of String, FieldInfo)

    Sub OnSetField(fieldName As String, newFieldValue As Object, oldFieldValue As Object, fromDbLoading As Boolean)
	'Sub SetField(fieldName As String, fieldValue As Object, Optional checkBusinessRule As Boolean = False)
    Sub SetFieldInternal(fieldInfo As FieldInfo, fieldValue As Object, fromDbLoading As Boolean, Optional checkBusinessRule As Boolean = False, Optional raisePropertyChangeEvent As Boolean = True)


    Sub RestoreField(fieldName As String)
    Sub RestoreToOriginlaFieldsAll()
    Function GetField(fieldName As String) As Object
    Function GetOriginalField(fieldName As String) As Object


    Function IsFieldChanged(fieldName As String) As Boolean

    'Clear fields
    Sub ClearFields()
    'Clear this entity and Child entities
    Sub ClearAll()

    'property is writable?
    Function IsPropertyWritable(ByVal propertyName As String) As Boolean

    'decide UI is readonly/writable or disabled/enabled
    'This only blocks UI entry, code still can change property if IsPropertyWritable is set to true
    Function IsPropertyUiWritable(propertyName As String) As Boolean

    'Load multiple entities
    Function LoadEntities(sql As String, Optional entityList As IBindingList = Nothing) As IBindingList
    Function LoadEntities(command As System.Data.IDbCommand, Optional entityList As IBindingList = Nothing) As IBindingList
    Function LoadEntities(Optional entityList As IBindingList = Nothing) As IBindingList
    Function LoadEntitiesById(Optional entityList As IBindingList = Nothing) As IBindingList

    'Load single entity
    Function LoadMeByPk(Optional throwNoEntityFoundException As Boolean = False, Optional throwMultiEntityFoundException As Boolean = False) As Boolean
    Function LoadMeById(Optional throwNoEntityFoundException As Boolean = False, Optional throwMultiEntityFoundException As Boolean = False) As Boolean

    'Called when data is starting loading
    Sub OnDataLoading()
    'Called when data is loaded from database into enitity
    Sub OnDataLoaded()


    'Called by DataLoaded, user can override this 
    'to set default values for entity
    Sub SetDefaultValues()

    'child entity operations
    Sub AddChildEntity(child As IEntity)
    Sub ClearChildEntities()


    'Called by CheckBusinessRules
    Function CheckMyBusinessRules(Optional force As Boolean = False) As Boolean
    ReadOnly Property MyBusinessRuleCheckNeeded As Boolean

    ReadOnly Property IsMeDirty() As Boolean

    'Support Child Entity Load/Unload operation
    Sub LoadChildEntities()
    Sub UnLoadChildEntities()

    'Normalize values stored in the map for convinience
    Function NormalizeValue(fieldName As String, fieldValue As Object) As Object

    Sub RaiseAllPropertyChange()
    Sub RaisePropertyChange(propName As String)

    ReadOnly Property IsCheckingRule() As Boolean

#Region "State Management"
    'State management
    Property IsNew() As Boolean
    Property IsDeleted() As Boolean
    ReadOnly Property IsDeleteCommitted() As Boolean
    ReadOnly Property IsModified() As Boolean
    ReadOnly Property IsDiscarded() As Boolean
    'Property IsNavigatingAway() As Boolean

    Sub ResetState()
#End Region

End Interface
