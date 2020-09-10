
Public Class EntityPasteErrorMap

    Private _errorMap As New Dictionary(Of IEntity, FieldMessageMap)

    Public ReadOnly Property ErrorEntities As IEnumerable(Of IEntity)
        Get
            Return _errorMap.Keys
        End Get
    End Property

    Default Public ReadOnly Property Item(ent As IEntity) As IEnumerable(Of FieldMessage)
        Get
            If _errorMap.ContainsKey(ent) Then
                Return _errorMap(ent).FieldMessageList()
            End If

            Return Nothing
        End Get
    End Property

    Public Sub AddErrorField(ent As IEntity, fieldName As String, errorMsg As String)
        If Not _errorMap.ContainsKey(ent) Then
            _errorMap.Add(ent, New FieldMessageMap())
        End If

        Dim curMap As FieldMessageMap = _errorMap(ent)
        curMap.AddFieldMessage(fieldName, errorMsg)
    End Sub

    Public Sub AddErroFieldList(ent As IEntity, fieldList As IEnumerable(Of FieldMessage))
        If fieldList.Count > 0 Then
            If Not _errorMap.ContainsKey(ent) Then
                _errorMap.Add(ent, New FieldMessageMap)
            End If

            Dim curMap As FieldMessageMap = _errorMap(ent)
            curMap.Add(fieldList)
        End If
    End Sub

    Public Sub Clear()
        _errorMap.Clear()
    End Sub

End Class
