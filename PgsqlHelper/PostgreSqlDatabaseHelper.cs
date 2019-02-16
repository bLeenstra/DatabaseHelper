using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace DatabaseHelper.Postgre
{
    public class PostgreSqlDatabaseHelper : BaseDatabaseHelper
    {
        private readonly NpgsqlConnection _connection;
        private readonly bool _disposeConnection;
        private NpgsqlTransaction _transaction;

        public PostgreSqlDatabaseHelper(NpgsqlConnection connection)
        {
            _connection = connection;
            _disposeConnection = false;
        }

        public PostgreSqlDatabaseHelper(NpgsqlConnectionStringBuilder connectionBuilder)
        {
            _connection = new NpgsqlConnection(connectionBuilder.ConnectionString);
            _disposeConnection = true;
        }

        public PostgreSqlDatabaseHelper(string host, ushort port, string username, string password) : this(
            new NpgsqlConnectionStringBuilder {Host = host, Port = port, Username = username, Password = password})
        {
        }

        public override bool TransactionBegin()
        {
            ValidateConnection();
            if (_transaction != null)
            {
                if (!_transaction.IsCompleted) return false;
                _transaction.Dispose();
            }

            _transaction = _connection.BeginTransaction();
            return true;
        }

        public override bool TransactionRollback()
        {
            if (_transaction == null || _transaction.IsCompleted) return false;
            _transaction.Rollback();
            return true;
        }

        public override bool TransactionCommit()
        {
            if (_transaction == null || _transaction.IsCompleted) return false;
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

        /// <summary>
        ///     PgSQL doesn't have "id's"
        ///     Use RETURNING After INSERT statements to get last inserted id
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override long QueryInsertedId(string query, params IDataParameter[] parameters)
        {
            throw new NotImplementedException();
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

        protected virtual NpgsqlCommand CreateCommandExplicit(string query, params IDataParameter[] parameters)
        {
            return (NpgsqlCommand) CreateCommand(query, parameters);
        }

        protected override IDbCommand CreateCommand(string query, params IDataParameter[] parameters)
        {
            var command = new NpgsqlCommand
            {
                CommandText = query,
                Connection = _connection
            };
            if (_transaction != null && !_transaction.IsCompleted) command.Transaction = _transaction;
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
            if (!_transaction.IsCompleted) _transaction.Rollback();
            _transaction.Dispose();
        }

        public sealed override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}