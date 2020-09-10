''' <summary>
''' This is a class keeping information for a business entity field.
''' A field is a class/object property with ColumnMapping attribute specified.
''' </summary>
''' <remarks>
''' FieldInfo contains following info for a field:
''' 1. field name, 
''' 2. field data type, 
''' 3. field index (used as a order to store field data for a business entity),
''' 4. column name (the column name specified in the ColumnMapping attribute),
''' 5. field type check (a boolean value indicates if type check should be conducted when field data changes 
''' to ensure it's the right data type for this field. User can specify this value in ColumnMapping attribute).
''' </remarks>
Public Class FieldInfo

    Protected _fieldName As String
    Protected _fieldType As Type
    Protected _fieldIndex As Integer
    Protected _columnName As String
    Protected _fieldTypeCheck As Boolean

    Public Sub New(fieldName As String, fieldType As Type, fieldIndex As Integer, columnName As String, Optional fieldTypeCheck As Boolean = True)
        _fieldName = fieldName
        _fieldType = fieldType
        _fieldIndex = fieldIndex
        _columnName = columnName

        If fieldType Is GetType(Boolean) Or fieldType Is GetType(Nullable(Of Boolean)) Then
            'Database does not have boolean type, don't check
            _fieldTypeCheck = False
        Else
            _fieldTypeCheck = fieldTypeCheck
        End If
    End Sub

    Public ReadOnly Property FieldName() As String
        Get
            Return _fieldName
        End Get
    End Property

    Public ReadOnly Property FieldType() As Type
        Get
            Return _fieldType
        End Get
    End Property

    Public ReadOnly Property FieldIndex() As Integer
        Get
            Return _fieldIndex
        End Get
    End Property

    Public ReadOnly Property columnName() As String
        Get
            Return _columnName
        End Get
    End Property

    Public ReadOnly Property FieldTypeCheck() As Boolean
        Get
            Return _fieldTypeCheck
        End Get
    End Property

End Class
