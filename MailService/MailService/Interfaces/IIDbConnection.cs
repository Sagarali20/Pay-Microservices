using System.Data;

namespace MailService.Interfaces
{
    public interface IIDbConnection
    {
        IDbConnection CreateConnection();
    }
}
