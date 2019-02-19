using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseHelperExtension
{
    public class DataRecord
    {
        internal int RowIndex;
        public DataResults parent;

        public static implicit operator bool(DataRecord dataRecord)
        {
            return dataRecord != null;
        }

        public T Get<T>(int index)
        {
            return (T)this[index];
        }
        public object Get(int index)
        {
            return this[index];
        }

        public T Get<T>(string name)
        {
            return (T)this[name];
        }

        public object Get(string name)
        {
            return this[name];
        }

        public object this[int index]
        {
            get
            {
                var result = parent.List[RowIndex + index];
                if (result == DBNull.Value)
                    return null;
                return result;
            }
        }

        public object this[string name]
        {
            get
            {
                var index = parent.NameIndex[name];
                var result = parent.List[RowIndex + index];
                if (result == DBNull.Value)
                    return null;
                return result;
            }
        }
    }
}
