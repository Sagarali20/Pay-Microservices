using MailService.Models;
using System.Data.SqlClient;

namespace pay_at.Interfaces
{
    public interface IMailLogService
    {
        bool CreateMailLog(MailLog mailLog);
    }

}
