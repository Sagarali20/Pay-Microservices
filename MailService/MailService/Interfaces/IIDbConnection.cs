using System.Data;

namespace pay_at.Interfaces
{
    public interface IIDbConnection
    {
        IDbConnection CreateConnection();
    }
}
