Imports System.ComponentModel

Public Class KeyValueEntity

    Private _key As Object
    Private _value As Object
    Private _desc As Object
    Private _tag As Object

    Public Sub New()

    End Sub

    Public Sub New(k As Object)
        Me.New(k, k)
    End Sub

    Public Sub New(k As Object, v As Object, Optional d As Object = Nothing)
        _key = k
        _value = v
        _desc = d
    End Sub

    Public Property Key() As Object
        Get
            Return _key
        End Get
        Set(ByVal v As Object)
            _key = v
        End Set
    End Property

    Public Property Value() As Object
        Get
            Return _value
        End Get
        Set(ByVal v As Object)
            If IsDBNull(v) Then
                _value = Nothing
            Else
                _value = v
            End If

        End Set
    End Property

    Public Property Desc() As Object
        Get
            Return _desc
        End Get
        Set(ByVal v As Object)
            If IsDBNull(v) Then
                _desc = Nothing
            Else
                _desc = v
            End If

        End Set
    End Property

    Public Property Tag() As Object
        Get
            Return _tag
        End Get
        Set(ByVal v As Object)
            If IsDBNull(v) Then
                _tag = Nothing
            Else
                _tag = v
            End If

        End Set
    End Property

End Class
