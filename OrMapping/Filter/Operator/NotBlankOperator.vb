Public Class NotBlankOperator
    Implements IFilterOperator


    Public Function AllowValueCount() As OpertatorValueCount Implements IFilterOperator.AllowValueCount
        Return OpertatorValueCount.NO_VALUE
    End Function

    Public Function AllowValueType(aType As System.Type) As Boolean Implements IFilterOperator.AllowValueType
        Return True
    End Function

    Public Function Evaluate(compareType As System.Type, value As Object, compareValue As Object) As Boolean Implements IFilterOperator.Evaluate
        If value Is Nothing Then
            Return False
        Else
            Return Not (value.ToString = "" Or value.ToString Is Nothing)
        End If
    End Function
End Class
