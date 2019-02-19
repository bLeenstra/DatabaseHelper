using System.Data;
using System.Data.Odbc;

namespace DatabaseHelper.Odbc
{
    public sealed class OdbcDatabaseHelperForceTransaction : OdbcDatabaseHelper
    {
        public OdbcDatabaseHelperForceTransaction(OdbcConnection connection) : base(connection)
        {
        }

        public OdbcDatabaseHelperForceTransaction(OdbcConnectionStringBuilder connectionBuilder) : base(
            connectionBuilder)
        {
        }

        protected override OdbcCommand CreateCommandExplicit(string query, params IDataParameter[] parameters)
        {
            TransactionBegin();
            return base.CreateCommandExplicit(query, parameters);
        }
    }
}