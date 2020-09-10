Imports System.ComponentModel

Public Class VirtualEntityInfo
    Inherits TypeDescriptionProvider
    Private _entityTypeMap As New Dictionary(Of String, VirtualEntityType)
    Private _propertyMap As New Dictionary(Of String, List(Of VirtualProperty))

    Private _virtualEntityType As Type
    Private _typeDescriptor As VirtualEntityTypeDescriptor

    Public Sub New(t As Type)
        _virtualEntityType = t
        _typeDescriptor = New VirtualEntityTypeDescriptor(Me)
        TypeDescriptor.AddProvider(Me, t)
    End Sub

    Public Overrides Function GetTypeDescriptor(objectType As System.Type, instance As Object) As ICustomTypeDescriptor
        If objectType = _virtualEntityType Then
            Return _typeDescriptor
        Else
            Return MyBase.GetTypeDescriptor(objectType, instance)
        End If
    End Function

    Public Sub AddType(entityType As Type, typeAlias As String)
        Dim virtualEntType As VirtualEntityType = New VirtualEntityType(entityType, typeAlias)
        For Each veType In _entityTypeMap.Values
            If veType.EntityType.Equals(entityType) Then
                Throw New Exception("Type already exists in VirtualEntityInfo: " & entityType.Name)
            End If
        Next

        If _entityTypeMap.ContainsKey(typeAlias) Then
            Throw New Exception("Type alias already exists in VirtualEntityInfo: " & typeAlias)
        Else
            _entityTypeMap.Add(typeAlias, virtualEntType)
            AddProperties(virtualEntType)
        End If

    End Sub

    Private Sub AddProperties(entityType As VirtualEntityType)
        For Each prop In TypeMap.GetTypeMap().GetPropertyTypeMap(entityType.EntityType)
            Dim propName As String = prop.Key
            Dim propType As Type = prop.Value
            Dim propList As List(Of VirtualProperty)

            If _propertyMap.ContainsKey(propName) Then
                propList = _propertyMap(propName)
            Else
                propList = New List(Of VirtualProperty)
                _propertyMap.Add(propName, propList)
            End If

            propList.Add(New VirtualProperty(entityType, propName, propType))
        Next
    End Sub

    Friend Function AllowType(aType As Type) As Boolean
        For Each entType In _entityTypeMap.Values
            If entType.EntityType.Equals(aType) Then
                Return True
            End If
        Next

        Return False
    End Function

    Friend Function GetEntityTypeByAlias(typeAlias As String) As Type
        If _entityTypeMap.ContainsKey(typeAlias) Then
            Return _entityTypeMap(typeAlias).EntityType
        End If

        Return Nothing
    End Function

    Friend Function GetAliasByEntityType(aType As Type) As String
        For Each v In _entityTypeMap
            If v.Value.EntityType.Equals(aType) Then
                Return v.Key
            End If
        Next

        Return Nothing
    End Function

    Friend Function GetPropertyListByName(propName As String) As List(Of VirtualProperty)
        If _propertyMap.ContainsKey(propName) Then
            Return _propertyMap(propName)
        End If

        Return Nothing
    End Function

    Friend Function GetPropetyFullName(aType As Type, propertyName As String) As String
        If Not AllowType(aType) Then
            Throw New Exception("Type not allowed in VirtualEntity: " & aType.Name)
        Else
            If IsUniqueProperty(propertyName) Then
                Return propertyName
            Else
                Return GetAliasByEntityType(aType) & "." & propertyName
            End If
        End If
    End Function

    Friend Function IsUniqueProperty(propName As String) As Boolean
        If _propertyMap.ContainsKey(propName) Then
            Return _propertyMap(propName).Count = 1
        Else
            Throw New Exception("Property Name not found in VirtualEntityInfo: " & propName)
        End If
    End Function

    Public Function GetPropertyDescriptors(attributes() As System.Attribute) As System.ComponentModel.PropertyDescriptorCollection
        Dim descriptors As List(Of PropertyDescriptor) = New List(Of PropertyDescriptor)

        For Each propName In _propertyMap.Keys
            Dim propList As List(Of VirtualProperty) = _propertyMap(propName)

            For Each prop In propList
                Dim componentType As Type = GetType(VirtualEntity)
                Dim propertyName As String = prop.PropertyName
                Dim propertyType As Type = prop.PropertyType
                Dim entType As Type = prop.EntityType.EntityType
                Dim isReadonly As Boolean = Not prop.EntityType.EntityType.GetProperty(prop.PropertyName).CanWrite

                descriptors.Add(New VirtualPropertyDescriptor(componentType, GetPropetyFullName(entType, propertyName), propertyType, isReadonly))
            Next
        Next

        Return New PropertyDescriptorCollection(descriptors.ToArray)
    End Function

End Class

