
<AttributeUsage(AttributeTargets.Property)> _
Public Class ColumnMappingAttribute
    Inherits Attribute

    Private _columnName As String
    Private _fieldTypeCheck As Boolean

    Public Sub New(columnName As String, Optional fieldTypeCheck As Boolean = True)
        _columnName = columnName
        _fieldTypeCheck = fieldTypeCheck
    End Sub

    Public Property ColumnName() As String
        Get
            Return _columnName
        End Get
        Set(ByVal value As String)
            _columnName = value
        End Set
    End Property

    Public ReadOnly Property FieldTypeCheck() As Boolean
        Get
            Return _fieldTypeCheck
        End Get
    End Property
End Class
