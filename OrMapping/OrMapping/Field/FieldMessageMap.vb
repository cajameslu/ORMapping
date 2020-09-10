''' <summary>
''' This class keeps a map of all FieldMessages for a BusinessEntity.
''' </summary>
''' <remarks>
''' The map key is the name of the field. The map value is a FieldMessage for this field.
''' </remarks>
Public Class FieldMessageMap
    Protected _map As New MyDictionary(Of String, FieldMessage)

    Public Sub AddFieldMessage(fieldName As String, msg As String)
        Dim fieldError As FieldMessage
        If Not _map.ContainsKey(fieldName) Then
            _map.Add(fieldName, New FieldMessage(fieldName))
        End If

        fieldError = _map(fieldName)
        fieldError.AddMessage(msg)
    End Sub

    Public Sub RemoveFieldMessage(fieldName As String)
        If _map.ContainsKey(fieldName) Then
            _map.Remove(fieldName)
        End If
    End Sub

    Public Sub AddFieldMessage(fieldMessage As FieldMessage)
        If Not _map.ContainsKey(fieldMessage.FieldName) Then
            _map.Add(fieldMessage.FieldName, fieldMessage)
        Else
            Dim fieldMsg As FieldMessage = _map(fieldMessage.FieldName)
            fieldMsg.AddMessage(fieldMessage.MessageList)
        End If
    End Sub

    Public Sub Add(fieldMsgList As IEnumerable(Of FieldMessage))
        For Each fieldMsg In fieldMsgList
            AddFieldMessage(fieldMsg)
        Next
    End Sub

    Public Function FieldMessageList() As IEnumerable(Of FieldMessage)
        Return _map.Values
    End Function

    Public Function FieldNameList() As IEnumerable(Of String)
        Return _map.Keys
    End Function

    Public Sub Clear()
        _map.Clear()
    End Sub

    Default Public ReadOnly Property Item(FieldName As String) As FieldMessage
        Get
            Return _map(FieldName)
        End Get
    End Property

    Public Function Count() As Integer
        Return _map.Count
    End Function

    ''' <summary>
    ''' Return a field message string for the specifed field.
    ''' </summary>
    ''' <param name="fieldName">Field name</param>
    Public Function GetMessage(fieldName As String) As String
        Dim fieldMessage As FieldMessage = _map(fieldName)
        If fieldMessage IsNot Nothing Then
            Return fieldMessage.Messages
        Else
            Return ""
        End If
    End Function

End Class
