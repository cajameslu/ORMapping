Imports System.ComponentModel
Imports System.Threading
Imports System.Linq.Expressions
Imports System.Reflection

''' <summary>
''' A sub class of BindingList. Its containing type can be any type inherits IEntity. 
''' Right now there're two type of IEntity types:
''' 1. Business Entity classes, implemented IBusinessEntity
''' 1. BusinessBindingList (this class) itself, implemented IBusinessBindingList
''' Usually this class is used to house a collection of Business entities. 
''' But in some situation, it can be used to house a collection of Business binding list too.
''' </summary>
''' <typeparam name="T"></typeparam>
''' <remarks></remarks>

Public Class BusinessBindingList(Of T As {IEntity})
    Inherits BindingList(Of T)
    Implements IBusinessBindingList

    Private _parentEntity As IBusinessEntity
    Private _isReadOnly As Boolean

    Protected _filterList As New AndFilterList
    Protected _allEntityList As New List(Of T)
    Protected _sortedEntityList As New List(Of T)
    Protected _fitlering As Boolean = False
    Protected _isNavigationAway As Boolean = False

    Public Sub New(Optional parentEntity As IBusinessEntity = Nothing)
        _parentEntity = parentEntity
    End Sub

    Public Sub New(enumerable As IEnumerable(Of T))
        _allEntityList.AddRange(enumerable.ToList)
        _sortedEntityList.AddRange(enumerable.ToList)

        DoFilter()
    End Sub

    Public Property IsNavigatingAway() As Boolean Implements IEntity.IsNavigatingAway
        Get
            Return _isNavigationAway
        End Get
        Set(value As Boolean)
            _isNavigationAway = value
        End Set
    End Property

    Public Function GetBusinessRuleErrorMsgSet() As HashSet(Of String) Implements IEntity.GetBusinessRuleErrorMsgSet
        Dim errMsgSet As New HashSet(Of String)
        For Each ent In _allEntityList
            errMsgSet.UnionWith(ent.GetBusinessRuleErrorMsgSet)
        Next

        Return errMsgSet
    End Function

    Public Function GetBusinessRuleError() As String Implements IEntity.GetBusinessRuleError
        Dim allErrMsg As String = ""
        Dim errMsg As String = ""

        For Each ent As IEntity In _allEntityList
            errMsg = ent.GetBusinessRuleError
            If errMsg <> "" Then
                allErrMsg &= vbNewLine
                allErrMsg &= ent.GetEntityIdString() + errMsg
                allErrMsg &= vbNewLine
            End If
        Next

        Return allErrMsg
    End Function


    Public Function CheckBusinessRules(ByRef errMsg As String, Optional cascadeCheck As Boolean = True, Optional force As Boolean = False) As Boolean Implements IEntity.CheckBusinessRules
        Dim ret As Boolean = CheckBusinessRules(cascadeCheck, force)

        errMsg = GetBusinessRuleError()
        Return ret
    End Function

    Public Function CheckBusinessRules(Optional cascadeCheck As Boolean = True, Optional force As Boolean = False) As Boolean Implements IEntity.CheckBusinessRules
        ClearBusinessRules()
        Return CheckBusinessRulesInternal(cascadeCheck, force)
    End Function

    Protected Friend Function CheckBusinessRulesInternal(cascadeCheck As Boolean, Optional force As Boolean = False) As Boolean Implements IEntity.CheckBusinessRulesInternal
        Dim ret As Boolean = True

        For Each ent In _allEntityList

            ent.IsNavigatingAway = IsNavigatingAway
            If Not ent.CheckBusinessRulesInternal(cascadeCheck, force) Then
                ret = False
            End If
        Next
        Return ret
    End Function

    Public Sub ClearBusinessRules(Optional cascading As Boolean = True) Implements IEntity.ClearBusinessRules
        For Each ent In _allEntityList
            ent.ClearBusinessRules(cascading)
        Next
    End Sub

    Public Sub Save(Optional checkBusinessRules As Boolean = True, Optional cascading As Boolean = True) Implements IEntity.Save
        For Each ent In _allEntityList
            ent.Save(checkBusinessRules, cascading)
        Next
    End Sub

    Public Sub SaveInOneTransaction(checkBusinessRules As Boolean, Optional cascading As Boolean = True) Implements IBusinessBindingList.SaveInOneTransaction
        If checkBusinessRules Then
            Dim errMsg As String = Nothing
            If Not Me.CheckBusinessRules(errMsg) Then
                Throw New Exception("Business Rules check failed: " & vbCrLf & errMsg)
            End If
        End If

        Dim dbconfig As IDBConfig = GetDBConfig()
        If dbconfig IsNot Nothing Then
            Dim transMan As New TransactionManager(dbconfig)
            Try
                SaveInternal(transMan, cascading)

                transMan.Commit()
            Catch e As Exception
                transMan.RollBack()
                Throw e
            Finally
                transMan.CloseConnection()
            End Try
        End If
    End Sub


    Protected Friend Sub SaveInternal(transMan As TransactionManager, cascading As Boolean) Implements IEntity.SaveInternal
        For Each ent In _allEntityList
            ent.SaveInternal(transMan, cascading)
        Next
    End Sub

    Protected Friend Sub RestoreToOrignialFieldsAllRecursive() Implements IEntity.RestoreToOrignialFieldsAllRecursive
        For Each ent In _allEntityList
            ent.RestoreToOrignialFieldsAllRecursive()
        Next
    End Sub

    Public ReadOnly Property IsDirty() As Boolean Implements IEntity.IsDirty
        Get
            For Each ent In _allEntityList
                If ent.IsDirty Then
                    Return True
                End If
            Next
            Return False
        End Get
    End Property

    'Delete from database and remove it from list
    Public Sub Delete(entity As IEntity, cascadeDelete As Boolean) Implements IBusinessBindingList.Delete
        If _allEntityList.Contains(entity) Then
            entity.Delete(cascadeDelete)

            Me.Remove(entity)
        End If
    End Sub

    'Delete All entity from database
    Public Sub Delete(cascadeDelete As Boolean) Implements IEntity.Delete
        For Each ent In _allEntityList
            ent.Delete(cascadeDelete)
        Next

        Me.Clear()
    End Sub

    'Delete All entity from database
    Public Sub Delete(cascadeDelete As Boolean, inOneTransaction As Boolean) Implements IBusinessBindingList.Delete
        If inOneTransaction Then
            Dim dbconfig As IDBConfig = GetDBConfig()
            If dbconfig IsNot Nothing Then
                Dim transMan As New TransactionManager(dbconfig)
                Try
                    DeleteInternal(transMan, cascadeDelete)

                    transMan.Commit()
                    Me.Clear()
                Catch e As Exception
                    transMan.RollBack()
                    ResetDeletedFlag()
                    Throw e
                Finally
                    transMan.CloseConnection()
                End Try
            End If
        Else
            Me.Delete(cascadeDelete)
        End If
    End Sub

    Protected Friend Sub ResetDeletedFlag() Implements IEntity.ResetDeletedFlag
        For Each ent In _allEntityList
            ent.ResetDeletedFlag()
        Next
    End Sub

    Protected Friend Sub DeleteInternal(transMan As TransactionManager, cascadeDelete As Boolean) Implements IEntity.DeleteInternal
        For Each ent In _allEntityList
            ent.DeleteInternal(transMan, cascadeDelete)
        Next
    End Sub

    Public Property ParentEntity() As IBusinessEntity Implements IEntity.ParentEntity
        Get
            Return _parentEntity
        End Get
        Set(value As IBusinessEntity)
            _parentEntity = value

            For Each ent In _allEntityList
                ent.ParentEntity = value
            Next
        End Set
    End Property

    Public Function GetAncestorEntity(type As Type) As IBusinessEntity Implements IEntity.GetAncestorEntity
        Dim parent As IBusinessEntity = ParentEntity

        While parent IsNot Nothing
            If parent.GetType = type Then
                Return parent
            Else
                parent = parent.ParentEntity
            End If
        End While

        Return Nothing
    End Function

    Public Function GetDBConfig() As IDBConfig Implements IEntity.GetDBConfig
        For Each ent In _allEntityList
            Dim dbconfig As IDBConfig = ent.GetDBConfig
            If dbconfig IsNot Nothing Then
                Return dbconfig
            End If
        Next

        Return Nothing
    End Function

    'One level refresh
    Public Sub Refresh() Implements IEntity.Refresh
        For Each ent In _allEntityList
            ent.Refresh()
        Next
    End Sub

    'all level refresh
    Public Sub RefreshAll() Implements IEntity.RefreshAll
        For Each ent In _allEntityList
            ent.RefreshAll()
        Next
    End Sub

    Public Property IsReadOnly() As Boolean Implements IEntity.IsReadOnly
        Get
            Return _isReadOnly
        End Get
        Set(value As Boolean)
            _isReadOnly = value

            For Each item As T In _allEntityList
                item.IsReadOnly = value
            Next
        End Set
    End Property

    Public Overridable Function GetEntityIdString() As String Implements IBusinessBindingList.GetEntityIdString
        Return ""
    End Function

#Region "Filter"

    Protected Overrides Sub ClearItems()
        MyBase.ClearItems()
        _allEntityList.Clear()
        _sortedEntityList.Clear()
    End Sub

    Protected Overrides Sub InsertItem(index As Integer, item As T)
        Monitor.Enter(Me)

        Try
            If index < Me.Count Then
                'insert
                Dim curItemAtIndex As T = Me(index)
                Dim indexAll As Integer = _allEntityList.IndexOf(curItemAtIndex)
                Dim indexSort As Integer = _sortedEntityList.IndexOf(curItemAtIndex)

                _allEntityList.Insert(indexAll, item)
                _sortedEntityList.Insert(indexSort, item)
            Else
                'append
                _allEntityList.Add(item)
                _sortedEntityList.Add(item)
            End If


            item.ParentEntity = Me.ParentEntity
            item.IsReadOnly = Me.IsReadOnly

            MyBase.InsertItem(index, item)

        Finally
            Monitor.Exit(Me)
        End Try
    End Sub

    Protected Overrides Sub RemoveItem(index As Integer)
        Monitor.Enter(Me)

        Try
            If index < Me.Count And index >= 0 Then
                Dim item As T = Me(index)
                _allEntityList.Remove(item)
                _sortedEntityList.Remove(item)
                item.ParentEntity = Nothing

                MyBase.RemoveItem(index)
            End If

        Finally
            Monitor.Exit(Me)
        End Try
    End Sub

    Public Sub AddFilter(filter As IItemFilter) Implements IBusinessBindingList.AddFilter
        _filterList.AddFilter(filter)
    End Sub

    Public Sub ClearFilter() Implements IBusinessBindingList.ClearFilter
        _filterList.ClearFilter()
    End Sub

    Public Sub DoFilter() Implements IBusinessBindingList.DoFilter
        Monitor.Enter(Me)

        Try
            RaiseListChangedEvents = False
            _fitlering = True

            MyBase.ClearItems()

            Dim index As Integer = 0
            For i As Long = 0 To _sortedEntityList.Count - 1
                Dim item As T = _sortedEntityList(i)
                If _filterList.Match(item) Then
                    'call MyBase.InsertItem instead of Add to avoid get into Me.InsertItem again
                    MyBase.InsertItem(index, item)
                    index += 1
                End If
            Next
        Finally
            _fitlering = False
            Monitor.Exit(Me)

            RaiseListChangedEvents = True
            ResetBindings()
        End Try
    End Sub

#End Region

#Region "Sort"

    Private _sortDirection As ListSortDirection
    Private _sortProperty As PropertyDescriptor

    Protected Overrides Sub ApplySortCore(ByVal pdsc As PropertyDescriptor, ByVal Direction As ListSortDirection)
        _sortProperty = pdsc
        _sortDirection = Direction

        Dim PCom As New PCompare(Of T)(pdsc, Direction)
        _sortedEntityList.Sort(PCom)

        DoFilter()
    End Sub

    Protected Overrides Sub RemoveSortCore()
        _sortedEntityList.Clear()
        _sortedEntityList.AddRange(_allEntityList)

        DoFilter()
    End Sub

    Protected Overrides ReadOnly Property SupportsSortingCore() As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides ReadOnly Property SortDirectionCore() As ListSortDirection
        Get
            Return _sortDirection
        End Get
    End Property

    Protected Overrides ReadOnly Property SortPropertyCore() As PropertyDescriptor
        Get
            Return _sortProperty
        End Get
    End Property

#End Region

End Class

#Region " Property comparer "
Class PCompare(Of T)
    Implements IComparer(Of T)

    Private Property propDes As PropertyDescriptor
    Private Property sortDir As ListSortDirection

    Friend Sub New(ByVal SortProperty As PropertyDescriptor, ByVal SortDirection As ListSortDirection)
        propDes = SortProperty
        SortDir = SortDirection
    End Sub
    Friend Function Compare(ByVal x As T, ByVal y As T) As Integer Implements IComparer(Of T).Compare
        Return IIf(sortDir = ListSortDirection.Ascending, Comparer.[Default].Compare(propDes.GetValue(x),
                propDes.GetValue(y)), Comparer.[Default].Compare(propDes.GetValue(x),propDes.GetValue(y)))
    End Function
End Class
#End Region
