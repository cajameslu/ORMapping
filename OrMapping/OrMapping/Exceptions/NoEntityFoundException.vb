Public Class NoEntityFoundException
    Inherits AbstractEntityException

    Public Sub New(entityName As String)
        MyBase.New(entityName, "", "No Business Entitiy found for entity type " & entityName)
    End Sub

End Class
