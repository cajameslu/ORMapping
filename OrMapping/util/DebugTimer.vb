Public Class DebugTimer

    Private _startTime As Date

    Public Sub New()
        _startTime = Date.Now
    End Sub

    Public Sub Print(prefix As String)
        Dim endTime As Date = Date.Now
        Dim span As TimeSpan = endTime - _startTime
        Debug.Print(prefix & " : " & span.TotalMilliseconds)
    End Sub

End Class
