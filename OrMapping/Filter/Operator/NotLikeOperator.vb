Public Class NotLikeOperator
    Inherits SingleValueOperator

    Public Overrides Function Evaluate(compareType As Type, value As Object, compareValue As Object) As Boolean
        Return Not (value Like compareValue)
    End Function

    Public Overrides Function AllowValueType(aType As Type) As Boolean
        Return aType Is GetType(String)
    End Function

End Class
