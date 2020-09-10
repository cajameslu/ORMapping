Public Class EqualOperator
    Inherits SingleValueOperator

    Public Overrides Function Evaluate(compareType As Type, value As Object, compareValue As Object) As Boolean
        If compareType Is GetType(String) Then
            Return Object.Equals(CommonUtil.Nvl(value, ""), CommonUtil.Nvl(compareValue, ""))
        Else
            Return Object.Equals(value, compareValue)
        End If

    End Function

    Public Overrides Function AllowValueType(aType As Type) As Boolean
        'any type
        Return True
    End Function

End Class
