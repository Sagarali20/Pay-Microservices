using MailService.Interfaces;
using MailService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MailService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MailController(ILogger<MailController> logger, IMailServices mailServices) : ControllerBase
    {
        private readonly ILogger<MailController>? _logger = logger;
        private readonly IMailServices _mailServices = mailServices;

        [HttpPost]
        public IActionResult? SendMail(MailBody mailBody)
        {
            return mailServices.HandleMails(mailBody);
        }
    }
}
