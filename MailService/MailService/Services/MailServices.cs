using MailKit.Net.Smtp;
using MailKit.Security;
using MailService.Controllers;
using MailService.Interfaces;
using MailService.Models;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using pay_at.Constants;

namespace MailService.Services
{
    public class MailServices(IConfiguration config, ILogger<MailServices> logger) : IMailServices
    {
        private readonly MailSetting? mailSetting = config.GetSection("MailSetting").Get<MailSetting>();
        private readonly ILogger<MailServices> _logger = logger;
        private readonly bool checkCertificateRevocation = false;
        private readonly bool serverCertificateValidationCallback = true;


        public ObjectResult handleMails(MailBody mailBody)
        {
            MailLog log = new();

            if (mailBody.Subject == null || mailBody.ToMail == null)
            {
                _logger.LogWarning("Email subject or recipient cannot be empty.");
                log.

                return new(new
                {
                    ResponseCode = "INVALID_DATA",
                    message = "Email subject or recipient cannot be empty."
                })
                {
                    StatusCode = 422
                };
            }

            if (!mailBody.IsWithAttachment && !mailBody.IsWithCc && !mailBody.IsWithBcc) // fun: sendMail 
            {
                return sendMail(mailBody);
            }
            else if (mailBody.IsWithAttachment && !mailBody.IsWithCc && !mailBody.IsWithBcc) // fun: sendMailWithFile
            {

                if (mailBody.Files != null || mailBody.Files.Count <= 0)
                {
                    return sendMailWithFile(mailBody);
                }
                else
                {
                    _logger.LogWarning("Attachment Not Found");
                    return new(new
                    {
                        ResponseCode = "INVALID_DATA",
                        message = "Attachment Not Found"
                    })
                    {
                        StatusCode = 422
                    };
                }
            }
            else if (!mailBody.IsWithAttachment && mailBody.IsWithCc && !mailBody.IsWithBcc) // fun: sendMailWithCc
            {
                if (mailBody.CcEmail == null || mailBody.CcEmail.Count <= 0)
                    _logger.LogWarning("Cc recipient Not Found");

                return sendMailWithCc(mailBody);
            }
            else if (!mailBody.IsWithAttachment && !mailBody.IsWithCc && mailBody.IsWithBcc) // fun: sendMailWithBcc
            {
                if (mailBody.BccEmail == null || mailBody.BccEmail.Count <= 0)
                    _logger.LogWarning("Bcc recipient Not Found");

                return sendMailWithBcc(mailBody);
            }
            else if (mailBody.IsWithAttachment && mailBody.IsWithCc && !mailBody.IsWithBcc) // fun: sendCcMailWithFile
            {
                if (mailBody.CcEmail == null || mailBody.CcEmail.Count <= 0)
                    _logger.LogWarning("Bcc recipient Not Found");

                if (mailBody.Files == null || mailBody.Files.Count <= 0)
                {
                    return sendMailWithBcc(mailBody);
                }
                else
                {
                    _logger.LogWarning("Attachment Not Found");
                    return new(new
                    {
                        ResponseCode = "INVALID_DATA",
                        message = "Attachment Not Found"
                    })
                    {
                        StatusCode = 422
                    };
                }
            }
            else if (mailBody.IsWithAttachment && !mailBody.IsWithCc && mailBody.IsWithBcc) // fun: sendBccMailWithFile
            {
                if (mailBody.BccEmail == null || mailBody.BccEmail.Count <= 0)
                    _logger.LogWarning("Bcc recipient Not Found");
                if (mailBody.Files == null || mailBody.Files.Count <= 0)
                {
                    return sendMailWithBcc(mailBody);
                }
                else
                {
                    _logger.LogWarning("Attachment Not Found");
                    return new(new
                    {
                        ResponseCode = "INVALID_DATA",
                        message = "Attachment Not Found"
                    })
                    {
                        StatusCode = 422
                    };
                }
            }
            return null;
        }

        public ObjectResult? sendBccMailWithFile(MailBody mailBody)
        {
            ObjectResult? objectResult = null;
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(mailSetting.Name, mailSetting.MailFrom));

                foreach (var toMail in mailBody.ToMail)
                {
                    message.To.Add(new MailboxAddress(toMail.Key, toMail.Value));
                }

                foreach (var bccMail in mailBody.BccEmail)
                {
                    message.Bcc.Add(new MailboxAddress(bccMail.Key, bccMail.Value));
                }

                message.Subject = mailBody.Subject;

                var body = new TextPart("plain")
                {
                    Text = mailBody.Text
                };
                var multipart = new Multipart("mixed");
                multipart.Add(body);

                foreach (var attachment in mailBody.Files)
                {
                    byte[] fileBytes = Convert.FromBase64String(attachment.Base64);
                    var stream = new MemoryStream(fileBytes);

                    var mimePart = new MimePart(MailConstants.GetMimeType(attachment.Extension))
                    {
                        Content = new MimeContent(stream),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = attachment.FileName
                    };
                    multipart.Add(mimePart);
                }
                message.Body = multipart;
                using (var client = new SmtpClient())
                {

                    client.CheckCertificateRevocation = checkCertificateRevocation;
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => serverCertificateValidationCallback;
                    client.Connect(mailSetting.Host, mailSetting.Port);
                    client.Authenticate(mailSetting.MailFrom, mailSetting.Password);
                    client.Send(message);
                    client.Disconnect(true);

                }
                objectResult = new(new
                {
                    ResponseCode = "SUCCESS",
                    message = "Success! The email was delivered."
                })
                {
                    StatusCode = 250
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                objectResult = new(new
                {
                    ResponseCode = "SERVER_ERROR",
                    message = "Server does not accept mail."
                })
                {
                    StatusCode = 521
                };
            }
            return objectResult;
        }

        public ObjectResult? sendCcMailWithFile(MailBody mailBody)
        {
            ObjectResult? objectResult = null;
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(mailSetting.Name, mailSetting.MailFrom));

                foreach (var toMail in mailBody.ToMail)
                {
                    message.To.Add(new MailboxAddress(toMail.Key, toMail.Value));
                }

                foreach (var ccMail in mailBody.CcEmail)
                {
                    message.Cc.Add(new MailboxAddress(ccMail.Key, ccMail.Value));
                }

                message.Subject = mailBody.Subject;

                var body = new TextPart("plain")
                {
                    Text = mailBody.Text
                };
                var multipart = new Multipart("mixed");
                multipart.Add(body);

                foreach (var attachment in mailBody.Files)
                {
                    byte[] fileBytes = Convert.FromBase64String(attachment.Base64);
                    var stream = new MemoryStream(fileBytes);

                    var mimePart = new MimePart(MailConstants.GetMimeType(attachment.Extension))
                    {
                        Content = new MimeContent(stream),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = attachment.FileName
                    };
                    multipart.Add(mimePart);
                }
                message.Body = multipart;
                using (var client = new SmtpClient())
                {

                    client.CheckCertificateRevocation = checkCertificateRevocation;
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => serverCertificateValidationCallback;
                    client.Connect(mailSetting.Host, mailSetting.Port);
                    client.Authenticate(mailSetting.MailFrom, mailSetting.Password);
                    client.Send(message);
                    client.Disconnect(true);

                }
                objectResult = new(new
                {
                    ResponseCode = "SUCCESS",
                    message = "Success! The email was delivered."
                })
                {
                    StatusCode = 250
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                objectResult = new(new
                {
                    ResponseCode = "SERVER_ERROR",
                    message = "Server does not accept mail."
                })
                {
                    StatusCode = 521
                };
            }
            return objectResult;
        }

        public ObjectResult? sendMail(MailBody mailBody)
        {
            ObjectResult? objectResult = null;
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(mailSetting.Name, mailSetting.MailFrom));

                foreach (var toMail in mailBody.ToMail)
                {
                    message.To.Add(new MailboxAddress(toMail.Key, toMail.Value));
                }

                message.Subject = mailBody.Subject;

                message.Body = new TextPart("plain")
                {
                    Text = mailBody.Text
                };

                using (var client = new SmtpClient())
                {
                    client.CheckCertificateRevocation = checkCertificateRevocation;
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => serverCertificateValidationCallback;
                    client.Connect(mailSetting.Host, mailSetting.Port);
                    client.Authenticate(mailSetting.MailFrom, mailSetting.Password);
                    client.Send(message);
                    client.Disconnect(true);
                }

                objectResult = new(new
                {
                    ResponseCode = "SUCCESS",
                    message = "Success! The email was delivered."
                })
                {
                    StatusCode = 250
                };
            }
            catch (SmtpCommandException ex)
            {
                _logger.LogError(ex.ToString());
                objectResult = new(new
                {
                    ResponseCode = "SERVER_ERROR",
                    message = "Server does not accept mail."
                })
                {
                    StatusCode = 521
                };
            }
            catch (SmtpProtocolException ex)
            {
                _logger.LogError(ex.ToString());
                objectResult = new(new
                {
                    ResponseCode = "SERVER_ERROR",
                    message = "Server does not accept mail."
                })
                {
                    StatusCode = 521
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                objectResult = new(new
                {
                    ResponseCode = "SERVER_ERROR",
                    message = "Server does not accept mail."
                })
                {
                    StatusCode = 521
                };
            }

            return objectResult;
        }

        public ObjectResult? sendMailWithBcc(MailBody mailBody)
        {
            ObjectResult? objectResult = null;
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(mailSetting.Name, mailSetting.MailFrom));

                foreach (var toMail in mailBody.ToMail)
                {
                    message.To.Add(new MailboxAddress(toMail.Key, toMail.Value));
                }

                foreach (var bccMail in mailBody.BccEmail)
                {
                    message.Bcc.Add(new MailboxAddress(bccMail.Key, bccMail.Value));
                }

                message.Subject = mailBody.Subject;

                message.Body = new TextPart("plain")
                {
                    Text = mailBody.Text
                };

                using (var client = new SmtpClient())
                {

                    client.CheckCertificateRevocation = checkCertificateRevocation;
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => serverCertificateValidationCallback;
                    client.Connect(mailSetting.Host, mailSetting.Port);
                    client.Authenticate(mailSetting.MailFrom, mailSetting.Password);
                    client.Send(message);
                    client.Disconnect(true);

                }
                objectResult = new(new
                {
                    ResponseCode = "SUCCESS",
                    message = "Success! The email was delivered."
                })
                {
                    StatusCode = 250
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                objectResult = new(new
                {
                    ResponseCode = "SERVER_ERROR",
                    message = "Server does not accept mail."
                })
                {
                    StatusCode = 521
                };
            }
            return objectResult;
        }

        public ObjectResult? sendMailWithCc(MailBody mailBody)
        {
            ObjectResult? objectResult = null;
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(mailSetting.Name, mailSetting.MailFrom));

                foreach (var toMail in mailBody.ToMail)
                {
                    message.To.Add(new MailboxAddress(toMail.Key, toMail.Value));
                }

                foreach (var ccMail in mailBody.CcEmail)
                {
                    message.Cc.Add(new MailboxAddress(ccMail.Key, ccMail.Value));
                }

                message.Subject = mailBody.Subject;

                message.Body = new TextPart("plain")
                {
                    Text = mailBody.Text
                };

                using (var client = new SmtpClient())
                {
                    client.CheckCertificateRevocation = checkCertificateRevocation;
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => serverCertificateValidationCallback;
                    client.Connect(mailSetting.Host, mailSetting.Port);
                    client.Authenticate(mailSetting.MailFrom, mailSetting.Password);
                    client.Send(message);
                    client.Disconnect(true);
                }
                objectResult = new(new
                {
                    ResponseCode = "SUCCESS",
                    message = "Success! The email was delivered."
                })
                {
                    StatusCode = 250
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                objectResult = new(new
                {
                    ResponseCode = "SERVER_ERROR",
                    message = "Server does not accept mail."
                })
                {
                    StatusCode = 521
                };
            }
            return objectResult;
        }

        public ObjectResult? sendMailWithFile(MailBody mailBody)
        {
            ObjectResult? objectResult = null;
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(mailSetting.Name, mailSetting.MailFrom));

                foreach (var toMail in mailBody.ToMail)
                {
                    message.To.Add(new MailboxAddress(toMail.Key, toMail.Value));
                }

                message.Subject = mailBody.Subject;

                var body = new TextPart("plain")
                {
                    Text = mailBody.Text
                };
                var multipart = new Multipart("mixed")
                {
                    body
                };

                foreach (var attachment in mailBody.Files)
                {
                    byte[] fileBytes = Convert.FromBase64String(attachment.Base64);
                    var stream = new MemoryStream(fileBytes);

                    var mimePart = new MimePart(MailConstants.GetMimeType(attachment.Extension))
                    {
                        Content = new MimeContent(stream),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = attachment.FileName
                    };
                    multipart.Add(mimePart);
                }
                message.Body = multipart;
                using (var client = new SmtpClient())
                {
                    client.CheckCertificateRevocation = checkCertificateRevocation;
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => serverCertificateValidationCallback;
                    client.Connect(mailSetting.Host, mailSetting.Port);
                    client.Authenticate(mailSetting.MailFrom, mailSetting.Password);
                    client.Send(message);
                    client.Disconnect(true);
                }
                objectResult = new(new
                {
                    ResponseCode = "SUCCESS",
                    message = "Success! The email was delivered."
                })
                {
                    StatusCode = 250
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                objectResult = new(new
                {
                    ResponseCode = "SERVER_ERROR",
                    message = "Server does not accept mail."
                })
                {
                    StatusCode = 521
                };
            }
            return objectResult;
        }

        private MailLog getMailLog(MailBody mailBody, MailSetting mailSetting){
            MailLog mailLog = new MailLog();
            mailLog.MailFrom = mailSetting.MailFrom;
            mailLog.MailTo = JsonConvert.SerializeObject(mailBody.ToMail.ToList());
            mailLog.Text = mailBody.Text;
            mailLog.MailCc = JsonConvert.SerializeObject(mailBody.CcEmail.ToList());
            mailLog.MailBcc = JsonConvert.SerializeObject(mailBody.BccEmail.ToList());
            mailLog.Subject = mailBody.Subject;
            

            return mailLog;
        }
    }
}
