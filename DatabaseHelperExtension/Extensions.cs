using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseHelperExtension
{
    public static class Extensions
    {
        public static DateTime MinDate = new DateTime(1901, 01, 01);

        public static DateTime GetDate(this IDataRecord idata, int index)
        {
            DateTime value;
            if (idata.IsDBNull(index) || (value = idata.GetDateTime(index)) <= MinDate)
            {
                return DateTime.MinValue;
            }
            else
            {
                return value;
            }
        }

        public static bool GetBool(this IDataRecord idata, int index)
        {
            return idata.IsDBNull(index) ? false : idata.GetInt64(index) != 0;
        }

        public static int GetInt(this IDataRecord idata, int index)
        {
            if (idata.IsDBNull(index))
            {
                return 0;
            }
            else
            {
                return idata.GetInt32(index); //.CleanString();
            }
        }

        public static long GetLong(this IDataRecord idata, int index)
        {
            return idata.IsDBNull(index) ? default(long) :  idata.GetInt64(index);        
        }
        public static decimal GetDec(this IDataRecord idata, int index)
        {
            return idata.IsDBNull(index) ? default(decimal) : idata.GetDecimal(index);
        }
        
        internal static string CleanString(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value;
        }

        public static string GetStr(this IDataRecord idata, int index)
        {
            return idata.IsDBNull(index) ? 
                string.Empty : 
                    idata.GetString(index) is string value ? 
                        value.CleanString() : string.Empty;
        }
    }
}
