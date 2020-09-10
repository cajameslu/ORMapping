Public Class ValueConverter

    Public Shared Function ConvertNull(targetType As Type, srcValue As Object) As Object
        Dim targetValue As Object = srcValue

        If IsDBNull(srcValue) Then
            targetValue = Nothing
        ElseIf srcValue IsNot Nothing Then
            If targetType.IsValueType AndAlso srcValue.GetType Is GetType(String) AndAlso Trim(srcValue) = "" Then
                targetValue = Nothing
            End If
        End If

        Return targetValue
    End Function


    Public Shared Function Convert(srcValue As Object, targetType As Type) As Object
        If srcValue IsNot Nothing Then
            Dim toType As Type = targetType
            If IsNullableType(toType) Then
                toType = Nullable.GetUnderlyingType(toType)
            End If
            Return System.Convert.ChangeType(srcValue, toType)
        Else
            Return srcValue
        End If
    End Function

    Protected Shared Function IsNullableType(ByVal aType As Type) As Boolean
        Return (aType.IsGenericType) AndAlso (aType.GetGenericTypeDefinition() Is GetType(Nullable(Of )))
    End Function

    Public Shared Function IsNumericType(atype As Type) As Boolean
        If atype Is Nothing Then

            Return False
        End If

            Select Type.GetTypeCode(atype)

            Case TypeCode.Byte, TypeCode.Decimal, TypeCode.Double, TypeCode.Int16, TypeCode.Int32, TypeCode.Int64, _
             TypeCode.SByte, TypeCode.Single, TypeCode.UInt16, TypeCode.UInt32, TypeCode.UInt64
                Return True

            Case TypeCode.Object
                If atype.IsGenericType AndAlso atype.GetGenericTypeDefinition() Is GetType(Nullable(Of )) Then
                    Return IsNumericType(Nullable.GetUnderlyingType(atype))
                End If

                Return False
        End Select

        Return False
    End Function

End Class
