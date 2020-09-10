Public Interface IDataEntity
    Inherits IEntity

    Sub Clear()

	Sub SetField(fieldName As String, fieldValue As Object, Optional checkBusinessRule As Boolean = False)

    Function BulkSetFields(fieldNames As IEnumerable(Of String), fieldValues As IEnumerable(Of Object), Optional continueOnError As Boolean = True) As IEnumerable(Of FieldMessage)
    Function BusinessRuleErrorList() As IEnumerable(Of FieldMessage)

End Interface
