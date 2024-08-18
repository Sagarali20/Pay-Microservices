using MailService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MailService.Interfaces
{
    public interface IMailServices
    {
        MailLog SendMail(MailBody mailBody, MailLog mailLog);
        MailLog SendMailWithFile(MailBody mailBody, MailLog mailLog);
        MailLog SendMailWithCc(MailBody mailBody, MailLog mailLog);
        MailLog SendMailWithBcc(MailBody mailBody, MailLog mailLog);
        MailLog SendCcMailWithFile(MailBody mailBody, MailLog mailLog);
        MailLog SendBccMailWithFile(MailBody mailBody, MailLog mailLog);
        ObjectResult HandleMails(MailBody mailBody);
    }
}
