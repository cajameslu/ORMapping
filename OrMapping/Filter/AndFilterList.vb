Public Class AndFilterList
    Implements IItemFilter

    'All filters in this list have AND logical relationship
    'which means it is only return true when all filters in the list return true

    Protected _allFilters As New List(Of IItemFilter)

    Public Sub AddFilter(filter As IItemFilter)
        _allFilters.Add(filter)
    End Sub

    Public Sub ClearFilter()
        _allFilters.Clear()
    End Sub

    'Only return true when all filters return true
    Public Function Match(entity As Object) As Boolean Implements IItemFilter.Match
        For Each Filter As IItemFilter In _allFilters
            If Not Filter.Match(entity) Then
                Return False
            End If
        Next

        Return True
    End Function
End Class
