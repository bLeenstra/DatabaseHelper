using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DatabaseHelper.Postgre;

namespace DatabaseHelper.Sqlite
{
    public class SqliteDatabaseHelper : BaseDatabaseHelper
    {
        private readonly SQLiteConnection _connection;
        private readonly bool _disposeConnection;
        private SQLiteTransaction _transaction;

        public SqliteDatabaseHelper(SQLiteConnection connection)
        {
            _connection = connection;
            _disposeConnection = false;
        }

        public SqliteDatabaseHelper(SQLiteConnectionStringBuilder connectionBuilder)
        {
            _connection = new SQLiteConnection(connectionBuilder.ConnectionString);
            _disposeConnection = true;
        }

        public SqliteDatabaseHelper(string datasource) : this(
            new SQLiteConnectionStringBuilder { DataSource = datasource, Version = 3 }) {}

        public SqliteDatabaseHelper(string datasource, bool useUtf16Encoding, string password) : this(
            new SQLiteConnectionStringBuilder { DataSource = datasource, UseUTF16Encoding = useUtf16Encoding, Version = 3, 
                                                Password = password }){}



        public override bool TransactionBegin()
        {
            ValidateConnection();
            if (_transaction != null)
            {
                //already in transaction
                return false;
            }

            _transaction = _connection.BeginTransaction();
            return true;
        }

        public override bool TransactionRollback()
        {
            if (_transaction == null) return false;
            _transaction.Rollback();
            return true;
        }

        public override bool TransactionCommit()
        {
            if (_transaction == null) return false;
            _transaction.Commit();
            return true;
        }

        public override void QueryNone(string query, params IDataParameter[] parameters)
        {
            using (var command = CreateCommandExplicit(query, parameters))
            {
                command.ExecuteNonQuery();
            }
        }

        public override int QueryRowCount(string query, params IDataParameter[] parameters)
        {
            using (var command = CreateCommandExplicit(query, parameters))
            {
                return command.ExecuteNonQuery();
            }
        }
        
        public override long QueryInsertedId(string query, params IDataParameter[] parameters)
        {
            using (var command = CreateCommandExplicit(query, parameters))
            {
                command.ExecuteScalar();
                return command.Connection.LastInsertRowId;
            }
        }

        public override T QueryResultsSingle<T>(string query, params IDataParameter[] parameters)
        {
            using (var command = CreateCommandExplicit(query, parameters))
            {
                return (T) command.ExecuteScalar();
            }
        }


        public override IDataReader QueryResultsDataReader(string query, params IDataParameter[] parameters)
        {
            using (var command = CreateCommandExplicit(query, parameters))
            {
                return command.ExecuteReader(CommandBehavior.SequentialAccess);
            }
        }

        /// <summary>
        ///     Runs Query and maps the resulting data to an object
        /// </summary>
        /// <param name="query">A SQLite query</param>
        /// <param name="entityMapper">Your entity mapper should match the T type</param>
        /// <param name="parameters">Your SQLite Params, if any are to be used with your query</param>
        /// <typeparam name="T">The type of object your entity mapper will map to</typeparam>
        /// <returns>A list of mapped objects</returns>
        public override List<T> QueryResultsEntityMapper<T>(string query, IEntityMapper entityMapper,
            params IDbDataParameter[] parameters)
        {
            using (var command = CreateCommandExplicit(query, parameters))
            {
                using (var dataReader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    var results = new List<T>();
                    while (dataReader.Read())
                        try
                        {
                            var item = (T) entityMapper.Map(dataReader);
                            results.Add(item);
                        }
                        catch (InvalidCastException e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }

                    return results;
                }
            }
        }

        protected virtual SQLiteCommand CreateCommandExplicit(string query, params IDataParameter[] parameters)
        {
            return (SQLiteCommand) CreateCommand(query, parameters);
        }

        protected override IDbCommand CreateCommand(string query, params IDataParameter[] parameters)
        {
            var command = new SQLiteCommand
            {
                CommandText = query,
                Connection = _connection
            };
            if (_transaction != null) command.Transaction = _transaction;
            command.Parameters.AddRange(parameters);
            return command;
        }

        protected override void ValidateConnection()
        {
            if (_connection == null) throw new Exception("Connection Details Not valid");

            if (_connection.State == ConnectionState.Closed) _connection.Open();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposeConnection) _connection?.Dispose();
            _transaction?.Dispose();
        }

        public sealed override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}