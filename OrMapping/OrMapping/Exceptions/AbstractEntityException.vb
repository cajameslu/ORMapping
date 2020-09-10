Public Class AbstractEntityException
    Inherits Exception

    Protected _fieldName As String
    Protected _entityName As String

    Public Sub New(entityName As String, fieldName As String, msg As String)
        MyBase.New(msg)

        _fieldName = fieldName
        _entityName = entityName
    End Sub

    Public ReadOnly Property FieldName() As String
        Get
            Return _fieldName
        End Get
    End Property

    Public ReadOnly Property EntityName() As String
        Get
            Return _entityName
        End Get
    End Property
End Class
