''' <summary>
''' This class stores the field data for a BusinessEntity.
''' </summary>
''' <remarks>
''' The field data are stored in the order specified by FieldInfo.FieldIndex.
''' </remarks>
Public Class EntityData

    Private _fieldNameToFieldInfoMap As Dictionary(Of String, FieldInfo)
    Private _dataArray() As Object

    Public Sub New(fieldNameToFieldInfoMap As Dictionary(Of String, FieldInfo))
        _fieldNameToFieldInfoMap = fieldNameToFieldInfoMap
        _dataArray = New Object(fieldNameToFieldInfoMap.Count) {}
    End Sub

    Public Function Clear() As List(Of String)
        Dim changedFieldNames As New List(Of String)

        For Each field In _fieldNameToFieldInfoMap.Values()
            If _dataArray(field.FieldIndex) IsNot Nothing Then
                changedFieldNames.Add(field.FieldName)
            End If

            _dataArray(field.FieldIndex) = Nothing
        Next

        Return changedFieldNames
    End Function

    Public Function FieldNames() As Dictionary(Of String, FieldInfo).KeyCollection
        Return _fieldNameToFieldInfoMap.Keys()
    End Function

    Default Public Property Item(key As String) As Object
        Get
            If _fieldNameToFieldInfoMap.ContainsKey(key) Then
                Return _dataArray(_fieldNameToFieldInfoMap(key).FieldIndex)
            Else
                Return Nothing
            End If
        End Get
        Set(value As Object)
            If _fieldNameToFieldInfoMap.ContainsKey(key) Then
                _dataArray(_fieldNameToFieldInfoMap(key).FieldIndex) = value
            End If
        End Set
    End Property

    Default Public Property Item(index As Integer) As Object
        Get
            If index < _dataArray.Count Then
                Return _dataArray(index)
            Else
                Return Nothing
            End If
        End Get
        Set(value As Object)
            If index < _dataArray.Count Then
                _dataArray(index) = value
            End If
        End Set
    End Property

End Class
