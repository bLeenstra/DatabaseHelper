using System.Data;

namespace DatabaseHelper
{
    public interface IEntityMapper
    {
        object Map(IDataRecord record);
    }
}