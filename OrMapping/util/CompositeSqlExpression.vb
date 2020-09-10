''' <summary>
''' This class represent a composit SQL expression, like:
''' functional: to_date(x, y) or  sysdate
''' operational: x + y
''' This kind of SQL expressions don't need to be wrapped (put inside '')
''' </summary>
''' <remarks></remarks>
Public Class CompositeSqlExpression
    Private _expression As String

    Public Sub New(sqlExpression As String)
        _expression = sqlExpression
    End Sub

    Public Overrides Function ToString() As String
        Return _expression
    End Function

End Class
