Public Class BasicItemFilter
    Implements IItemFilter

    'Initial values passed in
    Protected _propertyName As String
    Protected _compareValue As Object
    Protected _filterOperator As IFilterOperator

    'Status 
    Protected _firstTimeCheck As Boolean = False
    Protected _validated As Boolean = False

    'Values used in calculation
    Protected _convertedCompareValue As Object = Nothing
    Protected _needConvertPropertyValue As Boolean = False
    Protected _compareType As Type = Nothing

    Public Sub New(propertyName As String, compareValue As Object, filterOperator As FilterOperators)
        Me.New(propertyName, compareValue, filterOperator.ToOperator)
    End Sub

    Public Sub New(propertyName As String, compareValue As Object, filterOperator As IFilterOperator)
        _propertyName = propertyName
        _compareValue = compareValue
        _filterOperator = filterOperator
    End Sub

    Protected Sub DecideCompareType(entity As Object)
        Dim propertyInfo As Reflection.PropertyInfo = entity.GetType.GetProperty(_propertyName)

        'first try to use property's data type
        _compareType = propertyInfo.PropertyType
        If Not _filterOperator.AllowValueType(_compareType) Then
            If _filterOperator.AllowValueType(GetType(String)) Then
                'Any type can be converted to string type
                'so if the property's type not supported
                'try string type
                _compareType = GetType(String)
                'remember every value from object's property should be converted to string
                _needConvertPropertyValue = True
            Else
                Throw New Exception("Operator does not support this type of value")
            End If
        End If

        If ValueConverter.IsNumericType(_compareType) Then
            'Use Decimal for all number type to keep the precision
            'like if property type is integer while compare value is double
            'If we convert compare value to integer, it loses its floating part
            'and return wrong result for equal operators for numbers like (1 vs 1.101)
            _compareType = GetType(Nullable(Of Decimal))
        End If
    End Sub

    Protected Sub ConvertCompareValue(entity As Object)
        If _filterOperator.AllowValueCount = OpertatorValueCount.NO_VALUE Then
            'no value needed, set to null
            _convertedCompareValue = Nothing
        ElseIf _filterOperator.AllowValueCount = OpertatorValueCount.SINGLE_VALUE Then
            'convert single value
            _convertedCompareValue = ValueConverter.Convert(_compareValue, _compareType)
        ElseIf _filterOperator.AllowValueCount = OpertatorValueCount.MULTI_VALUE Then
            'Convert list of values
            _convertedCompareValue = New List(Of Object)
            If _compareValue IsNot Nothing Then
                For Each v In _compareValue
                    _convertedCompareValue.add(ValueConverter.Convert(v, _compareType))
                Next
            End If
        Else
            Throw New Exception("Invalid AllowValueCount")
        End If
    End Sub

    Protected Sub FirstCheck(entity As Object)
        Try
            DecideCompareType(entity)
            ConvertCompareValue(entity)

            _validated = True
        Catch ex As Exception
            _validated = False
        Finally
            _firstTimeCheck = True
        End Try
    End Sub

    Public Function Match(entity As Object) As Boolean Implements IItemFilter.Match
        Try
            If Not _firstTimeCheck Then
                FirstCheck(entity)
            End If

            If _validated Then
                Dim propertyInfo As Reflection.PropertyInfo = entity.GetType.GetProperty(_propertyName)
                Dim value As Object = propertyInfo.GetValue(entity, Nothing)
                If _needConvertPropertyValue Then
                    'this only happens when operator does not allow property's type, but allow string type
                    value = ValueConverter.Convert(value, _compareType)
                End If
                Return _filterOperator.Evaluate(_compareType, value, _convertedCompareValue)
            Else
                'Filter not valid, not able to run it, assume to matched (as if this filter does not exist)
                Return True
            End If
        Catch ex As Exception
            'error occured, assume to matched (as if this filter does not exist)
            Return True
        End Try
    End Function

End Class
