Imports System.ComponentModel

Public Class VirtualEntityTypeDescriptor
    Inherits CustomTypeDescriptor

    Private _virtualEntityInfo As VirtualEntityInfo

    Public Sub New(ve As VirtualEntityInfo)
        _virtualEntityInfo = ve
    End Sub

#Region "CustomTypeDescriptor"

    Public Overrides Function GetProperties() As System.ComponentModel.PropertyDescriptorCollection
        Return GetProperties(Nothing)
    End Function

    Public Overrides Function GetProperties(attributes() As System.Attribute) As System.ComponentModel.PropertyDescriptorCollection
        Return _virtualEntityInfo.GetPropertyDescriptors(attributes)
    End Function

#End Region

End Class
