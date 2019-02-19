using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseHelper.Misc
{
    public class DataRecord
    {
        internal int RowIndex;
        public DataResults parent;

        public static implicit operator bool(DataRecord dataRecord)
        {
            return dataRecord != null;
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
