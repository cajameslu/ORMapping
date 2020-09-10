Public Interface IBusinessRules

    Sub AddSharedRuleUnit(ruleUnit As ISharedBusinessRuleUnit)
    Sub ClearSharedRuleUnits()

    ReadOnly Property BusinessEntity As IBusinessEntity

    Function CheckBusinessRules() As Boolean
    Sub RunBusinessRules()

    Function GetErrors() As String
    Function GetErrorSet() As HashSet(Of String)

    Function GetWarnings() As String
    Function GetWarningSet() As HashSet(Of String)

    Sub AddError(field As String, msg As String)
    Sub RemoveError(field As String)
    Sub ClearErrors()

    Sub AddWarning(field As String, msg As String)
    Sub ClearWarnings()

    Sub ClearAll()

    ReadOnly Property ErrorCount As Integer
    ReadOnly Property WarningCount As Integer

    Function GetError(propertyName As String) As String

    Function FieldErrorList() As IEnumerable(Of FieldMessage)

End Interface
