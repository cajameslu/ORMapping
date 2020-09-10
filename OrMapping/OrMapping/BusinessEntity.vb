''' <summary>
''' A sub class of BusinessEntityCore class, adding some utility functions.
''' </summary>
''' <remarks></remarks>
Public MustInherit Class BusinessEntity
    Inherits BusinessEntityCore

    Public Overridable Function CharToNullableBool(v As String, Optional trueFlag As String = "Y", Optional falseFlag As String = "N") As Nullable(Of Boolean)
        Return BooleanUtil.CharToNullableBool(v, trueFlag, falseFlag)
    End Function

    Public Overridable Function NullableBoolToChar(v As Nullable(Of Boolean), Optional trueFlag As String = "Y", Optional falseFlag As String = "N") As String
        Return BooleanUtil.NullableBoolToChar(v, trueFlag, falseFlag)
    End Function

    Public Overridable Function CharToBool(v As String, Optional trueFlag As String = "Y", Optional falseFlag As String = "N") As Boolean
        Return BooleanUtil.CharToBool(v, trueFlag, falseFlag)
    End Function

    Public Overridable Function BoolToChar(v As Boolean, Optional trueFlag As String = "Y", Optional falseFlag As String = "N") As String
        Return BooleanUtil.BoolToChar(v, trueFlag, falseFlag)
    End Function

    Public Overridable Function NumberToNullableBool(v As Nullable(Of Decimal)) As Nullable(Of Boolean)
        Return BooleanUtil.NumberToNullableBool(v)
    End Function

    Public Overridable Function NullableBoolToNumber(v As Nullable(Of Boolean)) As Nullable(Of Decimal)
        Return BooleanUtil.NullableBoolToNumber(v)
    End Function

    Public Shared Function Nvl(a As Object, b As Object) As Object
        Return CommonUtil.Nvl(a, b)
    End Function

    Public Overridable Function FormatToDate(ByVal pstrValue As String) As String
        Dim strResult As String = ""

        If pstrValue = "" Then
            strResult = Nothing
        Else
            strResult = Format(Convert.ToDateTime(pstrValue), GeneralDateFormat)
        End If

        Return strResult
    End Function

    Public Overridable Function GeneralDateFormat() As String
        Return "mm/dd/yyyy"
    End Function
End Class
