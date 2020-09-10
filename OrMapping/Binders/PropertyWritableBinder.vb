Imports System.ComponentModel
Imports cnrl.exploitation.common.ORMapping

Public Class PropertyWritableBinder
    Implements INotifyPropertyChanged

    Public Event PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs) _
        Implements INotifyPropertyChanged.PropertyChanged

    Private _propertyName As String
    Private _businessEntity As IBusinessEntity

    Public Sub New(propName As String)
        _propertyName = propName
    End Sub

    Private _isWritable As Boolean = True
    Public Property IsWritable() As Boolean
        Get
            Return _isWritable
        End Get
        Set(ByVal value As Boolean)
            Dim changed As Boolean = (value <> _isWritable)

            _isWritable = value

            If changed Then
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("IsWritable"))
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("IsReadOnly"))
            End If
        End Set
    End Property

    Public ReadOnly Property IsReadOnly()
        Get
            Return Not _isWritable
        End Get
    End Property

    Private Sub CheckUiWritableChange(entity As IBusinessEntity, propName As String)
        If entity IsNot Nothing Then
            If _propertyName.Equals(propName) Then
                IsWritable = entity.IsPropertyUiWritable(_propertyName)
            End If
        Else
            'Entity is null, hence it's not writable
            IsWritable = False
        End If
    End Sub

    Public Sub BindTo(entity As IBusinessEntity)
        If _businessEntity IsNot Nothing Then
            RemoveHandler _businessEntity.UiWritableChanged, AddressOf CheckUiWritableChange
        End If

        _businessEntity = entity

        If entity IsNot Nothing Then
            AddHandler entity.UiWritableChanged, AddressOf CheckUiWritableChange
        End If

        'initial check
        CheckUiWritableChange(entity, _propertyName)
    End Sub

End Class

