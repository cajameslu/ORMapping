Imports System.ComponentModel

Public Class VirtualPropertyDescriptor
    Inherits PropertyDescriptor

    Private _componentType As Type
    Private _propertyType As Type
    Private _isReadonly As Boolean

    Public Sub New(componentType As Type, name As String, propertyType As Type, isReadonly As Boolean)
        MyBase.new(name, Nothing)

        _componentType = componentType
        _propertyType = propertyType
        _isReadonly = isReadonly
    End Sub

    Public Overrides Function CanResetValue(component As Object) As Boolean
        Return False
    End Function

    Public Overrides ReadOnly Property ComponentType As Type
        Get
            Return _componentType
        End Get
    End Property

    Public Overrides Function GetValue(component As Object) As Object
        Return CType(component, VirtualEntity).GetPropertyValue(Name)
    End Function

    Public Overrides ReadOnly Property IsReadOnly As Boolean
        Get
            Return _isReadonly
        End Get
    End Property

    Public Overrides ReadOnly Property PropertyType As Type
        Get
            Return _propertyType
        End Get
    End Property

    Public Overrides Sub ResetValue(component As Object)

    End Sub

    Public Overrides Sub SetValue(component As Object, value As Object)
        CType(component, VirtualEntity).SetPropertyValue(Name, value)
    End Sub

    Public Overrides Function ShouldSerializeValue(component As Object) As Boolean
        Return True
    End Function

End Class
