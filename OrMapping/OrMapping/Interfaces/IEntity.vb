Public Interface IEntity

    'Save entity to database
    Sub Save(Optional checkBusinessRules As Boolean = True, Optional cascading As Boolean = True)
    Sub SaveInternal(transMan As TransactionManager, cascading As Boolean)

    Sub Delete(cascadeDelete As Boolean)

    Sub DeleteInternal(transMan As TransactionManager, cascadeDelete As Boolean)
    Sub ResetDeletedFlag()

    'If entity is dirty: new created, or deleted, or modified
    ReadOnly Property IsDirty() As Boolean
    Property IsNavigatingAway() As Boolean

    'Check business rules
    Function CheckBusinessRules(Optional cascadeCheck As Boolean = True, Optional force As Boolean = False) As Boolean
    Function CheckBusinessRulesInternal(cascadeCheck As Boolean, Optional force As Boolean = False) As Boolean
    Function CheckBusinessRules(ByRef errMsg As String, Optional cascadeCheck As Boolean = True, Optional force As Boolean = False) As Boolean
    Function GetBusinessRuleError() As String
    Function GetBusinessRuleErrorMsgSet() As HashSet(Of String)

    Sub ClearBusinessRules(Optional cascading As Boolean = True)

    'Parent entity
    Property ParentEntity() As IBusinessEntity
    Function GetAncestorEntity(type As Type) As IBusinessEntity

    'Whole entity is read only?
    Property IsReadOnly() As Boolean

    Function GetDBConfig() As IDBConfig

    Sub Refresh()
    Sub RefreshAll()

    Sub RestoreToOrignialFieldsAllRecursive()

    Function GetEntityIdString() As String
End Interface
