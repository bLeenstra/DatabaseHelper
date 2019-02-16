using System.Data;
using System.Data.SQLite;

namespace DatabaseHelper.Sqlite
{
    public sealed class SqliteDatabaseHelperForceTransaction : SqliteDatabaseHelper
    {

        protected override SQLiteCommand CreateCommandExplicit(string query, params IDataParameter[] parameters)
        {
            TransactionBegin();
            return base.CreateCommandExplicit(query, parameters);
        }

        public SqliteDatabaseHelperForceTransaction(SQLiteConnection connection) : base(connection)
        {
        }

        public SqliteDatabaseHelperForceTransaction(SQLiteConnectionStringBuilder connectionBuilder) : base(connectionBuilder)
        {
        }

        public SqliteDatabaseHelperForceTransaction(string datasource) : base(datasource)
        {
        }

        public SqliteDatabaseHelperForceTransaction(string datasource, bool useUtf16Encoding, string password) : base(datasource, useUtf16Encoding, password)
        {
        }
    }
}