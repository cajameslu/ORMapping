Public Class MultiEntityFoundException
    Inherits AbstractEntityException

    Public Sub New(entityName As String)
        MyBase.New(entityName, "", "Multiple Business Entities Found for entity type " & entityName)
    End Sub

End Class
