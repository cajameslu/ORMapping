''' <summary>
''' TransactionManager is used to organized database operations in one transaction, either commit or roll back.
''' </summary>
''' <remarks>
''' TransactionManager creates a database connection and begin a transaction when it is created. 
''' In order to release database connection, user must make sure to call CloseConnection 
''' after calls Commit or Rollback, or when any exception happens.
''' </remarks>
Public Class TransactionManager

    Public Sub New(dbconfig As IDBConfig)
        _connection = dbconfig.CreateConnection
        _transaction = _connection.BeginTransaction
    End Sub

    Private _connection As IDbConnection

    Public ReadOnly Property Connection() As IDbConnection
        Get
            Return _connection
        End Get

    End Property


    Private _transaction As IDbTransaction

    Public ReadOnly Property Transaction() As IDbTransaction
        Get
            Return _transaction
        End Get

    End Property

    Public Sub RollBack()
        _transaction.Rollback()
    End Sub

    Public Sub Commit()
        _transaction.Commit()
    End Sub

    Public Sub CloseConnection()
        DBUtil.CloseConnection(_connection)
    End Sub
End Class
