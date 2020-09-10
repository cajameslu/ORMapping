Imports System.ComponentModel
Imports System.Data.OracleClient

Public Interface IBusinessDataAccess

    Function CreateSelectCommand() As System.Data.IDbCommand
    Function CreateSelectByPkCommand() As System.Data.IDbCommand
    Function CreateSelectByIdCommand() As System.Data.IDbCommand
    Function CreateInsertCommand() As System.Data.IDbCommand
    Function CreateUpdateCommand() As System.Data.IDbCommand
    Function CreateDeleteCommand() As System.Data.IDbCommand

    Function CreateConnection() As System.Data.IDbConnection
    Function GetDBConfig() As IDBConfig

    Function CreateEntity() As IBusinessEntity
    Function CreateEntityList() As IBindingList

    'Load Entities from database
    Function LoadEntities(command As IDbCommand, Optional entityList As IBindingList = Nothing) As IBindingList
    Function LoadEntities(sql As String, Optional entityList As IBindingList = Nothing) As IBindingList
    'Load one entity
    Function LoadSingleEntity(command As IDbCommand, entity As IBusinessEntity, Optional throwNoEntityFoundException As Boolean = False, Optional throwMultiEntityFoundException As Boolean = False) As Boolean

    'Massload validation list from DB
    'Function CreateEntityValidationList() As IBindingList
    'Function CreateValidatePreloadCommand() As System.Data.IDbCommand


    'Save entity to database
    Sub UpdateEntity(transMan As TransactionManager)
    Sub InsertEntity(transMan As TransactionManager)
    Sub DeleteEntity(transMan As TransactionManager)

    Function InsertCommandOutParamToPKMap() As String(,)

End Interface
