Public MustInherit Class SingleValueOperator
    Implements IFilterOperator


    Public Function AllowValueCount() As OpertatorValueCount Implements IFilterOperator.AllowValueCount
        Return OpertatorValueCount.SINGLE_VALUE
    End Function

    Public MustOverride Function Evaluate(compareType As System.Type, value As Object, compareValue As Object) As Boolean Implements IFilterOperator.Evaluate

    Public MustOverride Function AllowValueType(aType As Type) As Boolean Implements IFilterOperator.AllowValueType

End Class
