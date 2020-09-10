Imports System.Data.OracleClient
Imports System.Data

Public Class DBUtil

    Private _dbconfig As IDBConfig

    Public Sub New(dbconfig As IDBConfig)
        _dbconfig = dbconfig
    End Sub

    Protected Function GetDBConfig() As IDBConfig
        Return _dbconfig
    End Function

    Public Shared Sub SetParameterValue(ByVal command As IDbCommand, ByVal parameterName As String, ByVal value As Object)
        command.Parameters(parameterName).IsNullable = True
        If IsDBNull(value) Or value Is Nothing Then
            command.Parameters(parameterName).Value = System.DBNull.Value
        Else
            command.Parameters(parameterName).Value = value
        End If
    End Sub

    Private Function GetConnection() As System.Data.IDbConnection
        Return GetDBConfig.CreateConnection()
    End Function

    Public Shared Sub CloseConnection(connection As IDbConnection)
        If connection IsNot Nothing Then
            Try
                connection.Close()
            Catch ex As Exception

            End Try
        End If
    End Sub

    Public Function CreateCommand() As IDbCommand
        Return New OracleCommand()
    End Function

    Public Function CreateCommand(sql As String, Optional cmdType As CommandType = CommandType.Text) As IDbCommand
        If cmdType <> CommandType.Text Then
            'prefix schema name for table and stored procedure/functions/packages
            sql = GetDBConfig().DatabaseSchema() + "." + sql
        End If

        Dim command As IDbCommand = New OracleCommand(sql)
        command.CommandType = cmdType

        Return command
    End Function


    Public Shared Function CreateParameter(paramName As String, paramType As OracleType, Optional direction As ParameterDirection = ParameterDirection.Input, Optional size As Integer = 0) As OracleParameter
        If direction = ParameterDirection.Input Then
            Return New OracleParameter(paramName, paramType)
        ElseIf paramType = OracleType.Cursor Then
            Return New System.Data.OracleClient.OracleParameter(paramName, paramType, 0, direction, True, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing)
        Else
            Dim param As New OracleParameter(paramName, paramType, size)
            param.Direction = direction
            Return param
        End If
    End Function

    Private Shared Function WrapParam(param As Object) As String
        If param Is Nothing Then
        End If

        If IsNumeric(param) Then
            Return param.ToString
        ElseIf param.GetType().IsAssignableFrom(GetType(CompositeSqlExpression)) Then
            Return param.ToString
        Else
            Return "'" & param.ToString & "'"
        End If
    End Function

    Public Function PopulateCommandSql(cmd As String, Optional params As Object() = Nothing) As String
        cmd = "begin " & GetDBConfig.DatabaseSchema() & "." & cmd & "("
        If params IsNot Nothing AndAlso params.Length > 0 Then
            cmd &= WrapParam(params(0))
            For i As Integer = 1 To params.Length - 1
                cmd &= ", " & WrapParam(params(i))
            Next
        End If
        cmd &= "); end;"

        Return cmd
    End Function

    Public Sub ExecuteProcedure(procedureName As String, Optional params As Object() = Nothing)
        ExecuteCommand(PopulateCommandSql(procedureName, params), CommandType.Text)
    End Sub

    Public Sub ExecuteCommand(sql As String, Optional cmdType As CommandType = CommandType.Text)
        Dim command = CreateCommand(sql, cmdType)
        ExecuteCommand(command)
    End Sub

    Public Sub ExecuteCommand(command As IDbCommand)
        Dim connection As IDbConnection = GetConnection()

        Try
            command.Connection = connection
            command.ExecuteNonQuery()

        Catch e As Exception
            Throw New Exception(e.Message)
        Finally
            DBUtil.CloseConnection(connection)
        End Try
    End Sub

End Class
