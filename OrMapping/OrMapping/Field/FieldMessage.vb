''' <summary>
''' This is a class to store error or warning messages for BusinessEntity's field.
''' </summary>
''' <remarks>
''' It stores two things, field name and a list of messages (error or warning) for this field.
''' </remarks>
Public Class FieldMessage
    Protected _fieldName As String
    Protected _mssageList As New List(Of String)

    Public Sub New(fieldName As String)
        _fieldName = fieldName
    End Sub

    Public Sub New(fieldName As String, msgList As List(Of String))
        _fieldName = fieldName
        _mssageList = msgList
    End Sub

    Public Sub New(fieldName As String, msg As String)
        _fieldName = fieldName
        AddMessage(msg)
    End Sub

    Public Sub AddMessage(errMsg As String)
        _mssageList.Add(errMsg)
    End Sub

    Public Sub AddMessage(msgList As List(Of String))
        _mssageList.AddRange(msgList)
    End Sub

    Public ReadOnly Property FieldName As String
        Get
            Return _fieldName
        End Get
    End Property

    ''' <summary>
    ''' Return the message list.
    ''' </summary>
    Public ReadOnly Property MessageList As List(Of String)
        Get
            Return _mssageList
        End Get
    End Property

    ''' <summary>
    ''' Return all the messages in a concatenated string with each message separated by , .
    ''' </summary>
    Public ReadOnly Property Messages As String
        Get
            Dim ret As String = ""
            If _mssageList.Count > 0 Then
                ret = _mssageList(0)
                For i As Integer = 1 To _mssageList.Count - 1
                    '  ret &= "," & Environment.NewLine & _mssageList(i)
                    ret &= vbNewLine & _mssageList(i)
                Next
            End If

            Return ret
        End Get
    End Property
End Class
