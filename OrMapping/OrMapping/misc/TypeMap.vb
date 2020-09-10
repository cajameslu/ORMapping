''' <summary>
''' TypeMap class keeps information for all BusinessEntity classes. The information includes:
''' All FieldInfos (fields and related info) for a business entity. 
''' All Properties and their data types for business entity.
''' </summary>
''' <remarks>
''' For convenience, this class keeps 5 maps:
''' 1. Column name to FieldInfo map: 
'''    Column name is the value specified in ColumnMapping attribute for data field. 
''' 2. Field name to FieldInfo map:
'''    Field name is the field/property name in a BusinessEntity class
''' 3. Property name to its data type map
''' 4. Property name to its dependent Properties map
''' 5. Class type to its FieldInfos map
''' </remarks>
Public Class TypeMap
    Implements ITypeMap

    Private Shared _typeMap As New TypeMap()

    Friend Shared Function GetTypeMap() As TypeMap
        Return _typeMap
    End Function


    'Shared members
    Private _allClassColumnNameToFieldInfoMap As New Dictionary(Of Type, Dictionary(Of String, FieldInfo))
    Private _allClassFieldNameToFieldInfoMap As New Dictionary(Of Type, Dictionary(Of String, FieldInfo))

    Private _allClassPropertyTypeMap As New Dictionary(Of Type, Dictionary(Of String, Type))
    Private _allClassPropertyDependencyMap As New Dictionary(Of Type, Dictionary(Of String, HashSet(Of String)))

    Private _allClassFieldInfoMap As New Dictionary(Of Type, List(Of FieldInfo))

    ''' <summary>
    ''' Get all FieldInfos for a BusinessEntity type.
    ''' </summary>
    ''' <param name="type">BusinessEntity class's type</param>
    ''' <returns>a List of FieldInfo for the specified type</returns>
    Friend Function GetFieldInfos(type As Type) As List(Of FieldInfo)
        Dim myFields As List(Of FieldInfo)

        SyncLock type
            If _allClassFieldInfoMap.ContainsKey(type) Then
                'Already populated
                myFields = _allClassFieldInfoMap(type)
            Else
                'Populating
                myFields = New List(Of FieldInfo)
                _allClassFieldInfoMap.Add(type, myFields)

                Dim fieldIndex As Integer = 0
                For Each prop In type.GetProperties
                    Dim colMapAttributes As ColumnMappingAttribute() = prop.GetCustomAttributes(GetType(ColumnMappingAttribute), False)
                    If colMapAttributes.GetLength(0) > 0 Then
                        Dim colMapAttr As ColumnMappingAttribute = colMapAttributes(0)
                        myFields.Add(New FieldInfo(prop.Name, prop.PropertyType, fieldIndex, colMapAttr.ColumnName.ToUpper(), colMapAttr.FieldTypeCheck))
                        fieldIndex += 1
                    End If
                Next
            End If
        End SyncLock

        Return myFields
    End Function

    ''' <summary>
    ''' Get a column name to FieldInfo map for a BusinessEntity type.
    ''' </summary>
    ''' <param name="type">Business entity class type</param>
    ''' <returns>Return a column name to FieldInfo map for the business entity type.</returns>
    ''' <remarks></remarks>
    Friend Function GetColumnNameToFieldInfoMap(type As Type) As Dictionary(Of String, FieldInfo) Implements ITypeMap.GetColumnNameToFieldInfoMap
        Dim myColumnNameToFieldInfoMap As Dictionary(Of String, FieldInfo)

        SyncLock type
            If _allClassColumnNameToFieldInfoMap.ContainsKey(type) Then
                'Already populated
                myColumnNameToFieldInfoMap = _allClassColumnNameToFieldInfoMap(type)
            Else
                'Populating
                myColumnNameToFieldInfoMap = New Dictionary(Of String, FieldInfo)
                _allClassColumnNameToFieldInfoMap.Add(type, myColumnNameToFieldInfoMap)

                Dim myFields As List(Of FieldInfo) = GetFieldInfos(type)
                For Each field In myFields
                    myColumnNameToFieldInfoMap.Add(field.columnName, field)
                Next
            End If
        End SyncLock

        Return myColumnNameToFieldInfoMap
    End Function

    ''' <summary>
    ''' Get a field name to FieldInfo map for a business entity type.
    ''' </summary>
    ''' <param name="type">Business entity type</param>
    ''' <returns>Returns a field name to FieldInfo map</returns>
    Friend Function GetFieldNameToFieldInfoMap(type As Type) As Dictionary(Of String, FieldInfo) Implements ITypeMap.GetFieldNameToFieldInfoMap
        Dim myFieldNameToFieldInfoMap As Dictionary(Of String, FieldInfo)

        SyncLock Me.GetType
            If _allClassFieldNameToFieldInfoMap.ContainsKey(type) Then
                'Already populated
                myFieldNameToFieldInfoMap = _allClassFieldNameToFieldInfoMap(type)
            Else
                'Populating
                myFieldNameToFieldInfoMap = New Dictionary(Of String, FieldInfo)
                _allClassFieldNameToFieldInfoMap.Add(type, myFieldNameToFieldInfoMap)

                Dim myFields As List(Of FieldInfo) = GetFieldInfos(type)
                For Each field In myFields
                    myFieldNameToFieldInfoMap.Add(field.FieldName, field)
                Next
            End If
        End SyncLock

        Return myFieldNameToFieldInfoMap
    End Function

    ''' <summary>
    ''' Get a property name to its data type map for a business entity type.
    ''' </summary>
    ''' <param name="type">Business entity type</param>
    ''' <returns>Returns a property name to its data type map.</returns>
    Friend Function GetPropertyTypeMap(type As Type) As Dictionary(Of String, Type) Implements ITypeMap.GetPropertyTypeMap
        Dim myPropTypeMap As Dictionary(Of String, Type)

        SyncLock Me.GetType
            If _allClassPropertyTypeMap.ContainsKey(type) Then
                'Already populated
                myPropTypeMap = _allClassPropertyTypeMap(type)
            Else
                'Populating
                myPropTypeMap = New Dictionary(Of String, Type)
                _allClassPropertyTypeMap.Add(type, myPropTypeMap)

                For Each prop In type.GetProperties
                    myPropTypeMap.Add(prop.Name, prop.PropertyType)
                Next
            End If
        End SyncLock

        Return myPropTypeMap
    End Function

    ''' <summary>
    ''' Get a property name to its dependent property names map for a business entity type.
    ''' </summary>
    ''' <param name="type">Business entity type</param>
    ''' <returns>a property name to its dependent property names map</returns>
    ''' <remarks></remarks>
    Friend Function GetPropertyDependencyMap(type As Type) As Dictionary(Of String, HashSet(Of String)) Implements ITypeMap.GetPropertyDependencyMap
        Dim myPropertyDependencyMap As Dictionary(Of String, HashSet(Of String))

        SyncLock type
            If _allClassPropertyDependencyMap.ContainsKey(type) Then
                'Already populated
                myPropertyDependencyMap = _allClassPropertyDependencyMap(type)
            Else
                'Populating
                myPropertyDependencyMap = New Dictionary(Of String, HashSet(Of String))
                _allClassPropertyDependencyMap.Add(type, myPropertyDependencyMap)

                For Each prop In type.GetProperties
                    Dim depPropertyMapAttributes As ParentPropertyAttribute() = prop.GetCustomAttributes(GetType(ParentPropertyAttribute), False)
                    If depPropertyMapAttributes.GetLength(0) > 0 Then
                        Dim parentProps As String() = depPropertyMapAttributes(0).ParentProperties
                        For Each parentProp In parentProps
                            If Not myPropertyDependencyMap.ContainsKey(parentProp) Then
                                Dim list As New HashSet(Of String)
                                myPropertyDependencyMap.Add(parentProp, list)
                            End If
                            myPropertyDependencyMap(parentProp).Add(prop.Name)
                        Next
                    End If
                Next
            End If
        End SyncLock

        Return myPropertyDependencyMap
    End Function

    ''' <summary>
    ''' Get a collection of dependent property names given a business entity type and its property name.
    ''' </summary>
    ''' <param name="type">Business entity type</param>
    ''' <param name="propertyName">property name</param>
    ''' <returns>a collection of dependent property names</returns>
    ''' <remarks></remarks>
    Friend Function GetDependentProperties(type As Type, propertyName As String) As HashSet(Of String) Implements ITypeMap.GetDependentProperties
        Dim map As Dictionary(Of String, HashSet(Of String)) = GetPropertyDependencyMap(type)
        If map.ContainsKey(propertyName) Then
            Return map(propertyName)
        Else
            Return Nothing
        End If
    End Function
End Class
