Imports System.ComponentModel

Public Class CommonUtil

    Public Shared Function Nvl(a As Object, b As Object) As Object
        If a IsNot Nothing Then
            Return a
        Else
            Return b
        End If
    End Function


    Public Shared Function IsValueInBindingList(value As Object, entList As BindingList(Of KeyValueEntity)) As Boolean
        For Each ent In entList
            If Object.Equals(ent.Value, value) Then
                Return True
            End If
        Next

        Return False
    End Function

End Class
