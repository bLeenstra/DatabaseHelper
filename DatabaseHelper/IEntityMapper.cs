using System.Data;

namespace DatabaseHelper
{
    public interface IEntityMapper
    {
        T Map<T>(IDataRecord record);
    }
}