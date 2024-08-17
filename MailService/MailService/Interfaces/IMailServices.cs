using MailService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MailService.Interfaces
{
    public interface IMailServices
    {
        ObjectResult? sendMail(MailBody mailBody);
        ObjectResult? sendMailWithFile(MailBody mailBody);
        ObjectResult? sendMailWithCc(MailBody mailBody);
        ObjectResult? sendMailWithBcc(MailBody mailBody);
        ObjectResult? sendCcMailWithFile(MailBody mailBody);
        ObjectResult? sendBccMailWithFile(MailBody mailBody);
        ObjectResult handleMails(MailBody mailBody);
    }
}
