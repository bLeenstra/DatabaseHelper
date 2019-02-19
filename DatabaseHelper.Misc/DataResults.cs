using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc
{
    public class DataResults
    {
        private long _numberRows;
        public long NumberRows => _numberRows;

        public Dictionary<string, int> NameIndex = new Dictionary<string, int>();

        public List<object> List;

        public static DataResults GetDataResults(IDataReader reader, int expectedRows = 1)
        {
            var dataResults = new DataResults();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                dataResults.NameIndex.Add(reader.GetName(i), i);
            }
            if (reader is DbDataReader dataReader)
                if (!dataReader.HasRows)
                    return dataResults;

            int colCount = reader.FieldCount;

            if (expectedRows <= 0)
                expectedRows = 1;

            dataResults.List = new List<object>(expectedRows * colCount);

            while (reader.Read())
            {
                int index = dataResults.List.Count;
                for (int i = 0; i < colCount; i++)
                {
                    dataResults.List.Add(reader.GetValue(i));
                }
                dataResults.mysqlRows.AddLast(new DataRecord() { RowIndex = index, parent = dataResults });
            }
            dataResults._numberRows = dataResults.mysqlRows.Count;
            dataResults.currentRow = dataResults.mysqlRows.First;

            return dataResults;
        }
        public LinkedList<DataRecord> mysqlRows = new LinkedList<DataRecord>();
        private LinkedListNode<DataRecord> currentRow = null;

        public DataRecord FetchAssoc()
        {
            return (currentRow = currentRow.Next)?.Value;
        }

        public static implicit operator bool(DataResults dataResults)
        {
            return dataResults != null;
        }
    }
}
