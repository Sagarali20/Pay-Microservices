using MailService.Models;
using System.Data.SqlClient;

namespace MailService.Interfaces
{
    public interface IMailLogService
    {
        bool CreateMailLog(MailLog mailLog);
    }

}
