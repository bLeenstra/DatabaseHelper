using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseHelper.MSSql
{
    public class MSSQLDatabaseHelper : BaseDatabaseHelper
    {
        private readonly SqlConnection _connection;
        private readonly bool _disposeConnection;
        private SqlTransaction _transaction;

        public bool IsComplete = false;

        public MSSQLDatabaseHelper(SqlConnection connection)
        {
            _connection = connection;
            _disposeConnection = false;
        }

        public MSSQLDatabaseHelper(SqlConnectionStringBuilder connectionBuilder)
        {
            _connection = new SqlConnection(connectionBuilder.ConnectionString);
            _disposeConnection = true;
        }

        public MSSQLDatabaseHelper(string host, ushort port, string username, string password) : this(
            new SqlConnectionStringBuilder { DataSource = $"{host},{port}", UserID = username, Password = password})
        {
        }

        public override bool TransactionBegin()
        {
            ValidateConnection();
            if (_transaction != null)
            {
                if (!IsComplete) return false;
                _transaction.Dispose();
            }

            _transaction = _connection.BeginTransaction();
            return true;
        }

        public override bool TransactionRollback()
        {
            if (_transaction == null || IsComplete) return false;
            _transaction.Rollback();
            IsComplete = true;
            return true;
        }

        public override bool TransactionCommit()
        {
            if (_transaction == null || IsComplete) return false;
            _transaction.Commit();
            IsComplete = true;
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
                return (long)command.ExecuteScalar();                
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
        /// <param name="query">A PgSQL query</param>
        /// <param name="entityMapper">Your entity mapper should match the T type</param>
        /// <param name="parameters">Your PgSQL Params, if any are to be used with your query</param>
        /// <typeparam name="TInput">The type of object your entity mapper will map to</typeparam>
        /// <returns>A list of mapped objects</returns>
        public override List<TInput> QueryResultsEntityMapper<TInput>(string query, IEntityMapper entityMapper,
            params IDbDataParameter[] parameters)
        {
            using (var command = CreateCommandExplicit(query, parameters))
            {
                using (var dataReader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    var results = new List<TInput>();
                    while (dataReader.Read())
                        results.Add(
                                entityMapper.Map<TInput>(dataReader));
                    return results;
                }
            }
        }

        protected virtual SqlCommand CreateCommandExplicit(string query, params IDataParameter[] parameters)
        {
            return (SqlCommand) CreateCommand(query, parameters);
        }

        protected override IDbCommand CreateCommand(string query, params IDataParameter[] parameters)
        {
            var command = new SqlCommand
            {
                CommandText = query,
                Connection = _connection
            };
            if (_transaction != null && !IsComplete) command.Transaction = _transaction;
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
            if (_transaction == null) return;
            if (!IsComplete) _transaction.Rollback();
            _transaction.Dispose();
        }

        public sealed override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}