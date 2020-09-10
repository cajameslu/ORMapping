''' <summary>
''' A place hold entity is a place hold. It does not have any data/field itself.
''' Instead it is usually used to house other business entities as its children entity.
''' It is useful for modelling the business entities in a hierarchy in an easy 
''' understood way.
''' It is kind of similar to a folder. If business entity is a actually file, 
''' place hold entity can be seen as a folder.
''' </summary>
''' <remarks></remarks>
Public Class PlaceHoldEntity
    Inherits BusinessEntity

    Public Sub New()
        ResetState()
    End Sub

    Protected Overrides Function CreateBusinessRules() As IBusinessRules
        Return Nothing
    End Function

    Protected Overrides Function CreateDataAccess() As IBusinessDataAccess
        Return Nothing
    End Function

    Public Overrides ReadOnly Property IsMeDirty As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides Function LoadMeByPk(Optional throwNoEntityFoundException As Boolean = False, Optional throwMultiEntityFoundException As Boolean = False) As Boolean
        Return True
    End Function

End Class
