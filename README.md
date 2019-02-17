# DatabaseHelper
Library to help with database usage

Currently supports PostgreSQL And SQLite

Connect to database with
using(var db = new PLACEDATABASECLASSANDCONTEXTHERE()){}

Use TransactionBegin() / TransactionRollback() / TransactionCommit() to control the transaction state.
The force transaction copies of the classes will call TransactionBegin automatically

Use QueryNone / QueryRowCount / QueryInsertedId / QueryResultsSingle / QueryResultsDataReader / QueryResultsEntityMapper
to run queries on the database, the function name indicated what data is returned.

For QueryResultsEntityMapper you should create your own implementations of IEntityMapper