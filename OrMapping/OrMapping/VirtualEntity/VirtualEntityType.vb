Friend Class VirtualEntityType
    Private _entityType As Type
    Private _typeAlias As String

    Friend Sub New(entityType As Type, typeAlias As String)
        _entityType = entityType
        _typeAlias = typeAlias
    End Sub

    Friend ReadOnly Property EntityType As Type
        Get
            Return _entityType
        End Get
    End Property

    Friend ReadOnly Property TypeAlias As String
        Get
            Return _typeAlias
        End Get

    End Property

End Class
