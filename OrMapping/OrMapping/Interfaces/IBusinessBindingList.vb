Imports System.ComponentModel

Public Interface IBusinessBindingList
    Inherits IBindingList, IEntity

    Overloads Sub Delete(entity As IEntity, cascadeDelete As Boolean)
    Overloads Sub Delete(cascadeDelete As Boolean, inOneTransaction As Boolean)
    Overloads Sub SaveInOneTransaction(checkBusinessRulss As Boolean, Optional cascading As Boolean = True)

    Sub AddFilter(filter As IItemFilter)
    Sub ClearFilter()
    Sub DoFilter()

End Interface
