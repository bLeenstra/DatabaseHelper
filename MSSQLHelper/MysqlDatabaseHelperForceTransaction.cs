using System.Data;
using System.Data.SqlClient;

namespace DatabaseHelper.MSSql
{
    public sealed class MSSqlSqlDatabaseHelperForceTransaction : MSSQLDatabaseHelper
    {
        public MSSqlSqlDatabaseHelperForceTransaction(SqlConnection connection) : base(connection)
        {
        }

        public MSSqlSqlDatabaseHelperForceTransaction(SqlConnectionStringBuilder connectionBuilder) : base(
            connectionBuilder)
        {
        }

        public MSSqlSqlDatabaseHelperForceTransaction(string host, ushort port, string username, string password) :
            base(host, port, username, password)
        {
        }

        protected override SqlCommand CreateCommandExplicit(string query, params IDataParameter[] parameters)
        {
            TransactionBegin();
            return base.CreateCommandExplicit(query, parameters);
        }
    }
}