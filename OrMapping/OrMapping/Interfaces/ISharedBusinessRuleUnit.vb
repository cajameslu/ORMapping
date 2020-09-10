Public Interface ISharedBusinessRuleUnit
    Sub CheckRuleUnit()
    Sub RunRuleUnit()

    Sub AddError(errMsg As String)
    Sub AddWarning(errMsg As String)

    Property ParentBusinessRule As IBusinessRules
    Property PropertyName As String

End Interface
