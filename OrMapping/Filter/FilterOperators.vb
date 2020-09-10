Imports System.Runtime.CompilerServices

Public Enum FilterOperators
    EqualOperator
    NotEqualOperator

    ContainOperator
    NotContainOperator

    LikeOperator
    NotLikeOperator

    StartWithOperator
    EndWithOperator

    GreatOperator
    GreaterOrEqualOperator

    LessOperator
    LessOrEqualOperator

    InOperator
    NotInOperator

    BlankOperator
    NotBlankOperator
End Enum

Module FilterOperatorsExtension
    <Extension()>
    Function ToOperator(theEum As FilterOperators) As IFilterOperator
        Select Case theEum
            Case FilterOperators.EqualOperator
                Return New EqualOperator
            Case FilterOperators.NotEqualOperator
                Return New NotEqualOperator

            Case FilterOperators.ContainOperator
                Return New ContainOperator
            Case FilterOperators.NotContainOperator
                Return New NotContainOperator

            Case FilterOperators.LikeOperator
                Return New LikeOperator
            Case FilterOperators.NotLikeOperator
                Return New NotLikeOperator

            Case FilterOperators.StartWithOperator
                Return New StartWithOperator
            Case FilterOperators.EndWithOperator
                Return New EndWithOperator

            Case FilterOperators.GreatOperator
                Return New GreatOperator
            Case FilterOperators.GreaterOrEqualOperator
                Return New GreaterOrEqualOperator

            Case FilterOperators.LessOperator
                Return New LessOperator
            Case FilterOperators.LessOrEqualOperator
                Return New LessOrEqualOperator

            Case FilterOperators.InOperator
                Return New InOperator
            Case FilterOperators.NotInOperator
                Return New NotInOperator

            Case FilterOperators.BlankOperator
                Return New BlankOperator
            Case FilterOperators.NotBlankOperator
                Return New NotBlankOperator

        End Select

        Return Nothing
    End Function

End Module

