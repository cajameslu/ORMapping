Imports System.Data
Imports System.Data.OracleClient

Public Class Query
    Private _dbconfig As IDBConfig

    Public Sub New(dbconfig As IDBConfig)
        _dbconfig = dbconfig
    End Sub

    Public Function GetResultCount(sql As String) As Long
        Return GetResultCount(New DBUtil(_dbconfig).CreateCommand(sql))
    End Function

    Public Function GetResultCount(command As IDbCommand) As Long
        Dim connection As IDbConnection = _dbconfig.CreateConnection()
        Dim i As Long = 0

        Try
            command.Connection = connection
            Dim reader As IDataReader = command.ExecuteReader()

            While reader.Read()
                i += 1
            End While

            reader.Close()
        Finally
            DBUtil.CloseConnection(connection)
        End Try

        Return i
    End Function

    Public Function HasResult(sql As String) As Boolean
        Return HasResult(New DBUtil(_dbconfig).CreateCommand(sql))
    End Function

    Public Function HasResult(command As IDbCommand) As Boolean
        Dim connection As IDbConnection = _dbconfig.CreateConnection()
        Dim ret As Boolean = False

        Try
            command.Connection = connection
            Dim reader As IDataReader = command.ExecuteReader()

            If reader.Read() Then
                ret = True
            End If

            reader.Close()
        Finally
            DBUtil.CloseConnection(connection)
        End Try

        Return ret
    End Function

    Public Function ReadKeyValueEntity(sql As String, Optional cmdType As CommandType = CommandType.Text) As KeyValueEntity
        Return ReadSingleValue(New DBUtil(_dbconfig).CreateCommand(sql, cmdType))
    End Function

    Public Function ReadKeyValueEntity(command As IDbCommand) As KeyValueEntity
        Dim connection As IDbConnection = _dbconfig.CreateConnection()
        Dim ret As KeyValueEntity = Nothing

        Try
            command.Connection = connection
            Dim reader As IDataReader = command.ExecuteReader()

            If reader.Read() Then
                ret = New KeyValueEntity(reader(0), reader(1))
            End If

            reader.Close()
        Finally
            DBUtil.CloseConnection(connection)
        End Try

        Return ret
    End Function

    Public Function ReadSingleValue(sql As String, Optional cmdType As CommandType = CommandType.Text) As Object
        Return ReadSingleValue(New DBUtil(_dbconfig).CreateCommand(sql, cmdType))
    End Function

    Public Function ReadSingleValue(command As IDbCommand) As Object
        Dim connection As IDbConnection = _dbconfig.CreateConnection()
        Dim ret As Object = Nothing

        Try
            command.Connection = connection
            Dim reader As IDataReader = command.ExecuteReader()

            If reader.Read() Then
                ret = reader(0)
            End If

            reader.Close()
        Finally
            DBUtil.CloseConnection(connection)
        End Try

        Return ret
    End Function

    Public Function ReadData(sql As String, Optional cmdType As CommandType = CommandType.Text, Optional dt As DataTable = Nothing) As DataTable
        Return ReadData(New DBUtil(_dbconfig).CreateCommand(sql, cmdType), dt)
    End Function

    Public Function ReadData(command As IDbCommand, Optional dt As DataTable = Nothing) As DataTable
        If dt Is Nothing Then
            dt = New DataTable
        End If

        Dim connection As IDbConnection = _dbconfig.CreateConnection()
        Dim da As New Data.OracleClient.OracleDataAdapter(command)

        Try
            command.Connection = connection
            da.Fill(dt)
        Catch e As Exception
            Throw e
        Finally
            DBUtil.CloseConnection(connection)
        End Try

        Return dt
    End Function

End Class
