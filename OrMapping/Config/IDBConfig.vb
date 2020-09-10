Public Interface IDBConfig

    Function CreateConnection() As IDbConnection
    ReadOnly Property DatabaseSchema() As String
    ReadOnly Property LookupFilterSeprator() As String
    ReadOnly Property LookupFunction() As String

End Interface

