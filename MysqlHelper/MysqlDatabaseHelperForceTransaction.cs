using System.Data;
using MySql;
using MySql.Data.MySqlClient;

namespace DatabaseHelper.MSSql
{
    public sealed class MySqlSqlDatabaseHelperForceTransaction : MySqlDatabaseHelper
    {
        public MySqlSqlDatabaseHelperForceTransaction(MySqlConnection connection) : base(connection)
        {
        }

        public MySqlSqlDatabaseHelperForceTransaction(MySqlConnectionStringBuilder connectionBuilder) : base(
            connectionBuilder)
        {
        }

        public MySqlSqlDatabaseHelperForceTransaction(string host, ushort port, string username, string password) :
            base(host, port, username, password)
        {
        }

        protected override MySqlCommand CreateCommandExplicit(string query, params IDataParameter[] parameters)
        {
            TransactionBegin();
            return base.CreateCommandExplicit(query, parameters);
        }
    }
}