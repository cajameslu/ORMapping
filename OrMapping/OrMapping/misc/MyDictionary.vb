''' <summary>
''' Make a convenient sub class of Dictionary.
''' </summary>
''' <typeparam name="TKey"></typeparam>
''' <typeparam name="TValue"></typeparam>
''' <remarks></remarks>
Public Class MyDictionary(Of TKey, TValue)
    Inherits Dictionary(Of TKey, TValue)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(initialCapacity As Integer)
        MyBase.New(initialCapacity)
    End Sub

    Public Sub New(dic As MyDictionary(Of TKey, TValue))
        MyBase.new(dic)
    End Sub

    ''' <summary>
    ''' Shadow this functison. Maybe a bad idea, but it's really convenient.
    ''' </summary>
    ''' <param name="k">Key to search.</param>
    ''' <returns>Return mapped value if specified key found, else return nothing.</returns>
    ''' <remarks></remarks>
    Default Public Shadows Property Item(k As TKey) As TValue
        Get
            If MyBase.ContainsKey(k) Then
                Return MyBase.Item(k)
            Else
                Return Nothing
            End If
        End Get

        Set(value As TValue)
            MyBase.Item(k) = value
        End Set
    End Property

End Class
