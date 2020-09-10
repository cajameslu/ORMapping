Public Class ModifyReadonlyFieldException
    Inherits AbstractEntityException

    Public Sub New(entityName As String, fieldName As String)
        MyBase.New(entityName, fieldName, "Try to modify readonly field : " & entityName & "." & fieldName)
    End Sub

End Class
