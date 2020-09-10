Public Class CustomFilter
    Implements IItemFilter

    Public Delegate Function FilterItemDelegate(entity As Object) As Boolean

    Protected _filterFunc As FilterItemDelegate

    Public Sub New(filterFunc As FilterItemDelegate)
        _filterFunc = filterFunc
    End Sub

    Public Function Match(entity As Object) As Boolean Implements IItemFilter.Match
        Return _filterFunc(entity)
    End Function
End Class
