using System.Data;
using Npgsql;

namespace DatabaseHelper.Postgre
{
    public sealed class PostgreSqlDatabaseHelperForceTransaction : PostgreSqlDatabaseHelper
    {
        public PostgreSqlDatabaseHelperForceTransaction(NpgsqlConnection connection) : base(connection)
        {
        }

        public PostgreSqlDatabaseHelperForceTransaction(NpgsqlConnectionStringBuilder connectionBuilder) : base(
            connectionBuilder)
        {
        }

        public PostgreSqlDatabaseHelperForceTransaction(string host, ushort port, string username, string password) :
            base(host, port, username, password)
        {
        }

        protected override NpgsqlCommand CreateCommandExplicit(string query, params IDataParameter[] parameters)
        {
            TransactionBegin();
            return base.CreateCommandExplicit(query, parameters);
        }
    }
}