Public Class NoFieldFoundException
    Inherits AbstractEntityException

    Public Sub New(entityName As String, fieldName As String)
        MyBase.New(entityName, fieldName, "Field " & fieldName & " not found in Entity " & entityName)
    End Sub

End Class
