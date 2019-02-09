using System;
using System.Collections.Generic;
using System.Data;

namespace DatabaseHelper
{
    internal interface IDatabaseHelper : IDisposable
    {
        bool TransactionBegin();
        bool TransactionRollback();
        bool TransactionCommit();
        void QueryNone(string query, params IDataParameter[] parameters);
        int QueryRowCount(string query, params IDataParameter[] parameters);
        long QueryInsertedId(string query, params IDataParameter[] parameters);
        T QueryResultsSingle<T>(string query, params IDataParameter[] parameters);
        IDataReader QueryResultsDataReader(string query, params IDataParameter[] parameters);

        List<T> QueryResultsEntityMapper<T>(string query, IEntityMapper entityMapper,
            params IDbDataParameter[] parameters);
    }
}