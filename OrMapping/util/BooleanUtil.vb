Public Class BooleanUtil
    Public Shared Function ToBoolean(v As String) As Boolean
        If v Is Nothing Then
            Return False
        End If

        Select v.ToUpper()
            Case "Y", "YES", "TRUE"
                Return True
            Case Else
                Return False
        End Select

    End Function

    Public Shared Function ToNullableBoolean(v As String) As Nullable(Of Boolean)
        If v Is Nothing Then
            Return Nothing
        End If

        Select Case v.ToUpper()
            Case "Y", "YES", "TRUE"
                Return True
            Case Else
                Return False
        End Select

    End Function

    Public Shared Function CharToNullableBool(v As String, Optional trueFlag As String = "Y", Optional falseFlag As String = "N") As Nullable(Of Boolean)
        If v Is Nothing OrElse v.Length = 0 Then
            Return New Nullable(Of Boolean)
        ElseIf trueFlag.Equals(v) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function NullableBoolToChar(v As Nullable(Of Boolean), Optional trueFlag As String = "Y", Optional falseFlag As String = "N") As String
        If Not v.HasValue Then
            Return Nothing
        ElseIf v Then
            Return trueFlag
        Else
            Return falseFlag
        End If
    End Function

    Public Shared Function CharToBool(v As String, Optional trueFlag As String = "Y", Optional falseFlag As String = "N") As Boolean
        If v Is Nothing OrElse v.Length = 0 Then
            Return False
        ElseIf trueFlag.Equals(v) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function BoolToChar(v As Boolean, Optional trueFlag As String = "Y", Optional falseFlag As String = "N") As String
        If v Then
            Return trueFlag
        Else
            Return falseFlag
        End If
    End Function

    Public Shared Function NumberToNullableBool(v As Nullable(Of Decimal)) As Nullable(Of Boolean)
        If Not v.HasValue Then
            Return New Nullable(Of Boolean)
        ElseIf v = 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Shared Function NullableBoolToNumber(v As Nullable(Of Boolean)) As Nullable(Of Decimal)
        If Not v.HasValue Then
            Return New Nullable(Of Decimal)()
        ElseIf v Then
            Return -1
        Else
            Return 0
        End If
    End Function

End Class
