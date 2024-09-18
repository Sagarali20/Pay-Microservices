using MailKit.Net.Smtp;
using MailKit.Security;
using MailService.Constants;
using MailService.Controllers;
using MailService.Interfaces;
using MailService.Models;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Tnef;
using Newtonsoft.Json;


namespace MailService.Services
{
    public class MailServices(IConfiguration config, ILogger<MailServices> logger, IMailLogService mailLogService) : IMailServices
    {
        private readonly MailSetting mailSetting = config.GetSection("MailSetting").Get<MailSetting>().getCredentials(config["Encryption:Key"], config["Encryption:IV"]);
        private readonly ILogger<MailServices> _logger = logger;
        private readonly bool checkCertificateRevocation = false;
        private readonly bool serverCertificateValidationCallback = true;
        private readonly IMailLogService _mailLogService = mailLogService;


        public ObjectResult HandleMails(MailBody mailBody)
        {
            MailLog mailLog = new();

            if (mailBody.Subject == null || mailBody.ToMail == null)
            {
                _logger.LogWarning("Email subject or recipient cannot be empty.");
                mailLog = buildMailLog(mailBody, mailSetting, 1, "INVALID_DATA", "Email subject or recipient cannot be empty.", 0);

                mailLog.Response = new(new
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
                mailLog = SendMail(mailBody, mailLog);
            }
            else if (mailBody.IsWithAttachment && !mailBody.IsWithCc && !mailBody.IsWithBcc) // fun: sendMailWithFile
            {

                if (mailBody.Files != null || mailBody.Files.Count <= 0)
                {
                    mailLog = SendMailWithFile(mailBody, mailLog);
                }
                else
                {
                    _logger.LogWarning("Attachment Not Found");
                    mailLog = buildMailLog(mailBody, mailSetting, 1, "INVALID_DATA", "Attachment Not Found", 0);

                    mailLog.Response = new(new
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

                mailLog = SendMailWithCc(mailBody, mailLog);
            }
            else if (!mailBody.IsWithAttachment && !mailBody.IsWithCc && mailBody.IsWithBcc) // fun: sendMailWithBcc
            {
                if (mailBody.BccEmail == null || mailBody.BccEmail.Count <= 0)
                    _logger.LogWarning("Bcc recipient Not Found");

                mailLog = SendMailWithBcc(mailBody, mailLog);
            }
            else if (mailBody.IsWithAttachment && mailBody.IsWithCc && !mailBody.IsWithBcc) // fun: sendCcMailWithFile
            {
                if (mailBody.CcEmail == null || mailBody.CcEmail.Count <= 0)
                    _logger.LogWarning("Bcc recipient Not Found");

                if (mailBody.Files == null || mailBody.Files.Count <= 0)
                {
                    _logger.LogWarning("Attachment Not Found");
                    mailLog = buildMailLog(mailBody, mailSetting, 1, "INVALID_DATA", "Attachment Not Found", 0);
                    mailLog.Response = new(new
                    {
                        ResponseCode = "INVALID_DATA",
                        message = "Attachment Not Found"
                    })
                    {
                        StatusCode = 422
                    };
                    
                }
                else
                {
                    mailLog = SendCcMailWithFile(mailBody, mailLog);
                }
            }
            else if (mailBody.IsWithAttachment && !mailBody.IsWithCc && mailBody.IsWithBcc) // fun: sendBccMailWithFile
            {
                if (mailBody.BccEmail == null || mailBody.BccEmail.Count <= 0)
                    _logger.LogWarning("Bcc recipient Not Found");
                if (mailBody.Files == null || mailBody.Files.Count <= 0)
                {
                    _logger.LogWarning("Attachment Not Found");
                    mailLog = buildMailLog(mailBody, mailSetting, 1, "INVALID_DATA", "Attachment Not Found", 0);
                    mailLog.Response = new(new
                    {
                        ResponseCode = "INVALID_DATA",
                        message = "Attachment Not Found"
                    })
                    {
                        StatusCode = 422
                    };
                }
                else
                {
                    mailLog = SendBccMailWithFile(mailBody, mailLog);
                }
            }
            _mailLogService.CreateMailLog(mailLog);
            return mailLog.Response;
        }

        public MailLog SendBccMailWithFile(MailBody mailBody, MailLog mailLog)
        {
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
                mailLog = buildMailLog(mailBody, mailSetting, 0, null, null, 1);
                mailLog.Response = new(new
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
                _logger.LogError(ex.ToString());
                mailLog = buildMailLog(mailBody, mailSetting, 1, "SERVER_ERROR", ex.ToString(), 0);
                mailLog.Response = new(new
                {
                    ResponseCode = "SERVER_ERROR",
                    message = "Server does not accept mail."
                })
                {
                    StatusCode = 521
                };
            }
            return mailLog;
        }

        public MailLog SendCcMailWithFile(MailBody mailBody, MailLog mailLog)
        {
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
                mailLog = buildMailLog(mailBody, mailSetting, 0, null, null, 1);
                mailLog.Response = new(new
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
                _logger.LogError(ex.ToString());
                mailLog = buildMailLog(mailBody, mailSetting, 1, "SERVER_ERROR", ex.ToString(), 0);
                mailLog.Response = new(new
                {
                    ResponseCode = "SERVER_ERROR",
                    message = "Server does not accept mail."
                })
                {
                    StatusCode = 521
                };
            }
            return mailLog;
        }

        public MailLog? SendMail(MailBody mailBody, MailLog mailLog)
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
                mailLog = buildMailLog(mailBody, mailSetting, 0, null, null, 1);
                mailLog.Response = new(new
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
                mailLog = buildMailLog(mailBody, mailSetting, 1, "SERVER_ERROR", ex.ToString(), 0);
                mailLog.Response = new(new
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
                mailLog = buildMailLog(mailBody, mailSetting, 1, "SERVER_ERROR", ex.ToString(), 0);
                mailLog.Response = new(new
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
                mailLog = buildMailLog(mailBody, mailSetting, 1, "SERVER_ERROR", ex.ToString(), 0);
                mailLog.Response = new(new
                {
                    ResponseCode = "SERVER_ERROR",
                    message = "Server does not accept mail."
                })
                {
                    StatusCode = 521
                };
            }

            return mailLog;
        }

        public MailLog SendMailWithBcc(MailBody mailBody, MailLog mailLog)
        {
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
                mailLog = buildMailLog(mailBody, mailSetting, 0, null, null, 1);
                mailLog.Response = new(new
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
                _logger.LogError(ex.ToString());
                mailLog = buildMailLog(mailBody, mailSetting, 0, "SERVER_ERROR", ex.ToString(), 1);
                mailLog.Response = new(new
                {
                    ResponseCode = "SERVER_ERROR",
                    message = "Server does not accept mail."
                })
                {
                    StatusCode = 521
                };
            }
            return mailLog;
        }

        public MailLog SendMailWithCc(MailBody mailBody, MailLog mailLog)
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
                mailLog = buildMailLog(mailBody, mailSetting, 0, null, null, 1);
                mailLog.Response = new(new
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
                _logger.LogError(ex.ToString());
                mailLog = buildMailLog(mailBody, mailSetting, 1, "SERVER_ERROR", ex.ToString(), 0);

                mailLog.Response = new(new
                {
                    ResponseCode = "SERVER_ERROR",
                    message = "Server does not accept mail."
                })
                {
                    StatusCode = 521
                };
            }
            return mailLog;
        }

        public MailLog SendMailWithFile(MailBody mailBody, MailLog mailLog)
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
                mailLog = buildMailLog(mailBody, mailSetting, 0, null, null, 1);
                mailLog.Response = new(new
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
                _logger.LogError(ex.ToString());
                mailLog = buildMailLog(mailBody, mailSetting, 1, "SERVER_ERROR", ex.ToString(), 0);
                mailLog.Response = new(new
                {
                    ResponseCode = "SERVER_ERROR",
                    message = "Server does not accept mail."
                })
                {
                    StatusCode = 521
                };
            }
            return mailLog;
        }

        private MailLog buildMailLog(MailBody mailBody, MailSetting mailSetting, int isExceptionOccured, string? exceptionStatus, string? exceptionDetail, int isMailSent)
        {
            MailLog mailLog = new MailLog();
            mailLog.MailFrom = mailSetting.MailFrom;
            mailLog.MailTo = JsonConvert.SerializeObject(mailBody.ToMail.ToList());
            mailLog.Subject = mailBody.Subject;
            mailLog.Text = mailBody.Text;
            mailLog.MailCc = JsonConvert.SerializeObject(mailBody.CcEmail == null ? [] : mailBody.CcEmail.ToList());
            mailLog.MailBcc = JsonConvert.SerializeObject(mailBody.BccEmail == null ? [] :mailBody.BccEmail.ToList());
            mailLog.IsWithFiles = mailBody.IsWithAttachment ? 1 : 0;
            mailLog.IsWithCc = mailBody.IsWithCc ? 1 : 0;
            mailLog.IsWithBcc = mailBody.IsWithBcc ? 1 : 0;
            mailLog.IsExceptionOccured = isExceptionOccured;
            mailLog.ExceptionStatus = exceptionStatus;
            mailLog.ExceptionDetail = exceptionDetail;
            mailLog.IsMailSent = isMailSent;
            return mailLog;
        }
    }
}
