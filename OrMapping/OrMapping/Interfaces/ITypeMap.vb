Public Interface ITypeMap

    Function GetFieldNameToFieldInfoMap(type As Type) As Dictionary(Of String, FieldInfo)
    Function GetColumnNameToFieldInfoMap(type As Type) As Dictionary(Of String, FieldInfo)

    Function GetPropertyTypeMap(type As Type) As Dictionary(Of String, Type)
    Function GetPropertyDependencyMap(type As Type) As Dictionary(Of String, HashSet(Of String))
    Function GetDependentProperties(type As Type, propertyName As String) As HashSet(Of String)
   
End Interface
