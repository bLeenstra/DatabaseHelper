using System.Collections.Generic;
using System.Data;

namespace DatabaseHelper
{
    public abstract class BaseDatabaseHelper : IDatabaseHelper
    {
        public abstract void Dispose();

        public abstract bool TransactionBegin();
        public abstract bool TransactionRollback();
        public abstract bool TransactionCommit();

        public abstract void QueryNone(string query, params IDataParameter[] parameters);
        public abstract int QueryRowCount(string query, params IDataParameter[] parameters);
        public abstract long QueryInsertedId(string query, params IDataParameter[] parameters);
        public abstract T QueryResultsSingle<T>(string query, params IDataParameter[] parameters);
        public abstract IDataReader QueryResultsDataReader(string query, params IDataParameter[] parameters);

        public abstract List<T> QueryResultsEntityMapper<T>(string query, IEntityMapper entityMapper,
            params IDbDataParameter[] parameters);

        protected abstract IDbCommand CreateCommand(string query, IDataParameter[] parameters);
        protected abstract void ValidateConnection();
    }
}