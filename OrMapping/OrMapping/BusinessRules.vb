''' <summary>
''' It is the base class for all Business Rule classes.
''' It provides basic functions for business rule check.
''' </summary>
''' <remarks></remarks>
Public MustInherit Class BusinessRules
    Implements IBusinessRules

    Protected _fieldErrorMap As New FieldMessageMap
    Protected _fieldWarningMap As New FieldMessageMap

    Protected _businessEntity As IBusinessEntity

    Protected _sharedRuleUnits As List(Of ISharedBusinessRuleUnit)

    Public Sub New(bzEntity As IBusinessEntity)
        _businessEntity = bzEntity
    End Sub

    Public Overridable Function CheckBusinessRules() As Boolean Implements IBusinessRules.CheckBusinessRules
        ClearAll()
        RunBusinessRules()

        RunSharedRuleUnits()

        Return Me.ErrorCount = 0
    End Function

    Protected Overridable Sub RunBusinessRules() Implements IBusinessRules.RunBusinessRules
    End Sub

    Protected Overridable Sub RunSharedRuleUnits()
        If _sharedRuleUnits IsNot Nothing Then
            For Each ruleUnit In _sharedRuleUnits
                ruleUnit.CheckRuleUnit()
            Next

            'Other entity's property change might cause error for these properties
            'hence manually raise property change event to notifiy UI
            For Each ruleUnit In _sharedRuleUnits
                _businessEntity.RaisePropertyChange(ruleUnit.PropertyName)
            Next
        End If
    End Sub

    Public ReadOnly Property BusinessEntity As IBusinessEntity Implements IBusinessRules.BusinessEntity
        Get
            Return _businessEntity
        End Get
    End Property

    Public Sub AddSharedRuleUnit(ruleUnit As ISharedBusinessRuleUnit) Implements IBusinessRules.AddSharedRuleUnit
        If _sharedRuleUnits Is Nothing Then
            _sharedRuleUnits = New List(Of ISharedBusinessRuleUnit)
        End If

        ruleUnit.ParentBusinessRule = Me
        _sharedRuleUnits.Add(ruleUnit)
    End Sub

    Public Sub ClearSharedRuleUnits() Implements IBusinessRules.ClearSharedRuleUnits
        _sharedRuleUnits = Nothing
    End Sub

    Public Function GetErrorSet() As HashSet(Of String) Implements IBusinessRules.GetErrorSet
        Dim errSet As New HashSet(Of String)

        For Each fieldMsg As FieldMessage In _fieldErrorMap.FieldMessageList
            errSet.UnionWith(fieldMsg.MessageList)
        Next

        Return errSet
    End Function

    Public Function GetErrors() As String Implements IBusinessRules.GetErrors
        Dim errMsg As String = ""

        For Each msg As String In GetErrorSet()
            errMsg &= vbNewLine & msg
        Next

        Return errMsg
    End Function

    Public Function GetWarningSet() As HashSet(Of String) Implements IBusinessRules.GetWarningSet
        Dim warnSet As New HashSet(Of String)

        For Each fieldMsg As FieldMessage In _fieldWarningMap.FieldMessageList
            warnSet.UnionWith(fieldMsg.MessageList)
        Next

        Return warnSet
    End Function

    Public Function GetWarnings() As String Implements IBusinessRules.GetWarnings
        Dim warnMsg As String = ""

        For Each msg As String In GetWarningSet()
            warnMsg &= vbNewLine & msg
        Next

        Return warnMsg
    End Function

    Protected Sub AddError(field As String, msg As String) Implements IBusinessRules.AddError
        _fieldErrorMap.AddFieldMessage(field, msg)
    End Sub

    Protected Sub RemvoeError(field As String) Implements IBusinessRules.RemoveError
        _fieldErrorMap.RemoveFieldMessage(field)
    End Sub

    Protected Sub ClearErrors() Implements IBusinessRules.ClearErrors
        _fieldErrorMap.Clear()
    End Sub

    Protected Sub AddWarning(field As String, msg As String) Implements IBusinessRules.AddWarning
        _fieldWarningMap.AddFieldMessage(field, msg)
    End Sub

    Protected Sub ClearWarnings() Implements IBusinessRules.ClearWarnings
        _fieldWarningMap.Clear()
    End Sub

    Public Sub ClearAll() Implements IBusinessRules.ClearAll
        ClearErrors()
        ClearWarnings()
    End Sub

    Public ReadOnly Property ErrorCount As Integer Implements IBusinessRules.ErrorCount
        Get
            Return _fieldErrorMap.Count
        End Get
    End Property

    Public ReadOnly Property WarningCount As Integer Implements IBusinessRules.WarningCount
        Get
            Return _fieldWarningMap.Count
        End Get
    End Property

    Public Function GetError(propertyName As String) As String Implements IBusinessRules.GetError
        Return _fieldErrorMap.GetMessage(propertyName)
    End Function

    Public Function FieldErrorList() As IEnumerable(Of FieldMessage) Implements IBusinessRules.FieldErrorList
        Return _fieldErrorMap.FieldMessageList
    End Function
End Class
