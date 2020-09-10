Imports System.ComponentModel
Imports cnrl.exploitation.common.ORMapping

Public Class NullEntityBinder
    Implements INotifyPropertyChanged

    Public Event PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs) _
        Implements INotifyPropertyChanged.PropertyChanged

    Private _isEntityNull As Boolean = True
    Public Property IsEntityNull() As Boolean
        Get
            Return _isEntityNull
        End Get
        Set(ByVal value As Boolean)
            Dim changed As Boolean = (value <> _isEntityNull)

            _isEntityNull = value

            If changed Then
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("IsEntityNull"))
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("IsEntityNotNull"))
            End If

        End Set
    End Property

    Public ReadOnly Property IsEntityNotNull()
        Get
            Return Not _isEntityNull
        End Get
    End Property

    Public Sub BindTo(entity As IBusinessEntity)
        If entity Is Nothing Then
            IsEntityNull = True
        Else
            IsEntityNull = False
        End If
    End Sub

End Class
