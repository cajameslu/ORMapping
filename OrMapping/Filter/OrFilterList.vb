Public Class OrFilterList
    Implements IItemFilter

    'All filters in this list have OR logical relationship
    'which means it is return true when any filter in the list return true

    Protected _allFilters As New List(Of IItemFilter)

    Public Sub AddFilter(filter As IItemFilter)
        _allFilters.Add(filter)
    End Sub

    Public Sub ClearFilter()
        _allFilters.Clear()
    End Sub

    'return true when any filter return true
    Public Function Match(entity As Object) As Boolean Implements IItemFilter.Match
        For Each Filter As IItemFilter In _allFilters
            If Filter.Match(entity) Then
                Return True
            End If
        Next

        Return False
    End Function
End Class
