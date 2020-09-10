Public Class EndWithOperator
    Inherits SingleValueOperator


    Public Overrides Function Evaluate(compareType As System.Type, value As Object, compareValue As Object) As Boolean

        If value IsNot Nothing Then
            Return value.EndsWith(compareValue)
        Else
            Return False
        End If
    End Function

    Public Overrides Function AllowValueType(aType As Type) As Boolean
        Return aType Is GetType(String)
    End Function

End Class
