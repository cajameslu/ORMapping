Public MustInherit Class DBConfig
    Implements IDBConfig

    Public MustOverride Function CreateConnection() As IDbConnection Implements IDBConfig.CreateConnection
    Public MustOverride ReadOnly Property DatabaseSchema() As String Implements IDBConfig.DatabaseSchema
    Public MustOverride ReadOnly Property LookupFunction() As String Implements IDBConfig.LookupFunction

    Public Overridable ReadOnly Property LookupFilterSeprator() As String Implements IDBConfig.LookupFilterSeprator
        Get
            Return "<~$#*~>"
        End Get
    End Property

End Class
