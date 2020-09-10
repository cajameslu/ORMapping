Public Interface IFilterOperator

    Function AllowValueCount() As OpertatorValueCount
    Function AllowValueType(aType As Type) As Boolean

    Function Evaluate(compareType As Type, value As Object, compareValue As Object) As Boolean


End Interface
