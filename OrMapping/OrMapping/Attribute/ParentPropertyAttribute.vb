<AttributeUsage(AttributeTargets.Property)> _
Public Class ParentPropertyAttribute
    Inherits Attribute

    Private _parentProperties As String()

    Public Sub New(parentProperties As String())
        _parentProperties = parentProperties
    End Sub

    Public Property ParentProperties() As String()
        Get
            Return _parentProperties
        End Get
        Set(ByVal value As String())
            _parentProperties = value
        End Set
    End Property
End Class
