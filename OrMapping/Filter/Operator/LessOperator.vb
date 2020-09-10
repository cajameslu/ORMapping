Public Class LessOperator
    Inherits SingleValueOperator


    Public Overrides Function Evaluate(compareType As Type, value As Object, compareValue As Object) As Boolean
        If ValueConverter.IsNumericType(compareType) Then
            'compare as number
            If value IsNot Nothing And compareValue IsNot Nothing Then
                Return value < compareValue
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Overrides Function AllowValueType(aType As Type) As Boolean
        Return ValueConverter.IsNumericType(aType)
    End Function

End Class
