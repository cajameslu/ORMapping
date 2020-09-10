''' <summary>
''' The class is created for cross property/entity business rule check.
''' In some situation, user request the error message shown in a couple of properties.
''' For e.g., there're entity A, B, C.
''' A has properties P1 and P2. B has property P3. 
''' Assume there's a rule: If P1 = 1, then P2 must be in (5 , 6), and P3 must be in ("X", "Y").
''' User request if this rule breaks, error message needs to be displayed on all 3 properties.
''' 
''' In this situation, this class is a better place to put the business logic 
''' and shared by these two entities' Business Rule objects.
''' The purpose of this design is to avoid same logic/code being copy/paste to multiple places.
''' </summary>
''' <remarks></remarks>
Public Class SharedBusinessRuleUnit
    Implements ISharedBusinessRuleUnit

    Protected _parentBusinessRule As IBusinessRules
    Protected _propertyName As String

    Public Sub New()

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="parentBusienssRule">Parent Business Rule object</param>
    ''' <param name="propertyName">The property name of the Business Object that this rule unit is checked for</param>
    ''' <remarks></remarks>
    Public Sub New(parentBusienssRule As IBusinessRules, propertyName As String)
        _parentBusinessRule = parentBusienssRule
        _propertyName = propertyName
    End Sub

    Public Sub CheckRuleUnit() Implements ISharedBusinessRuleUnit.CheckRuleUnit
        RunRuleUnit()
    End Sub

    'Parent Business Rule object
    Public Property ParentBusinessRule As IBusinessRules Implements ISharedBusinessRuleUnit.ParentBusinessRule
        Get
            Return _parentBusinessRule
        End Get
        Set(value As IBusinessRules)
            _parentBusinessRule = value
        End Set
    End Property

    'The property name of the Business Object that this rule unit is checked for
    Public Property PropertyName As String Implements ISharedBusinessRuleUnit.PropertyName
        Get
            Return _propertyName
        End Get
        Set(value As String)
            _propertyName = value
        End Set
    End Property

    'override this to provide business logic
    Protected Overridable Sub RunRuleUnit() Implements ISharedBusinessRuleUnit.RunRuleUnit
        'if error occurs, call AddError to record error message
    End Sub

    Protected Overridable Sub AddError(errMsg As String) Implements ISharedBusinessRuleUnit.AddError
        'Ask parent BusinessRule to add error
        'it does not keep its own error collection for simplarity
        _parentBusinessRule.AddError(_propertyName, errMsg)
    End Sub

    Protected Overridable Sub AddWarning(errMsg As String) Implements ISharedBusinessRuleUnit.AddWarning
        'Ask parent BusinessRule to add warning
        'it does not keep its own warning collection for simplarity
        _parentBusinessRule.AddWarning(_propertyName, errMsg)
    End Sub

End Class
