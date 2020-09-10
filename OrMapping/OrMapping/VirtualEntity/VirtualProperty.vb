Public Class VirtualProperty
    Private _entityType As VirtualEntityType
    Private _propertyName As String
    Private _propertyType As Type

    Friend Sub New(entityType As VirtualEntityType, propertyName As String, propertyType As Type)
        _entityType = entityType
        _propertyName = propertyName
        _propertyType = propertyType
    End Sub

    Friend ReadOnly Property EntityType As VirtualEntityType
        Get
            Return _entityType
        End Get
    End Property

    Friend ReadOnly Property PropertyName As String
        Get
            Return _propertyName
        End Get

    End Property

    Friend ReadOnly Property PropertyType As Type
        Get
            Return _propertyType
        End Get

    End Property


End Class
