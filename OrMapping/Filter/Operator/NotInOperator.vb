Public Class NotInOperator
    Implements IFilterOperator


    Public Function Evaluate(compareType As Type, value As Object, compareValue As Object) As Boolean Implements IFilterOperator.Evaluate
        For Each v In compareValue
            If Object.Equals(value, v) Then
                Return False
            End If
        Next

        Return True
    End Function

    Public Function AllowValueCount() As OpertatorValueCount Implements IFilterOperator.AllowValueCount
        Return OpertatorValueCount.MULTI_VALUE
    End Function

    Public Function AllowValueType(aType As Type) As Boolean Implements IFilterOperator.AllowValueType
        'any type
        Return True
    End Function

End Class
