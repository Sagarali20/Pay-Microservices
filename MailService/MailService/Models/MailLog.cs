using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace MailService.Models
{
    public class MailLog
    {
        public Int64 Id { get; set; }
        public string? MailFrom { get; set; }
        public string? MailTo { get; set; }
        public string? Subject { get; set; }
        public string? Text { get; set; }
        public string? MailCc { get; set; }
        public string? MailBcc { get; set; }
        public int IsWithFiles { get; set; }
        public int IsWithCc { get; set; }
        public int IsWithBcc { get; set; }
        public int IsExceptionOccured { get; set; }
        public string? ExceptionStatus { get; set; }
        public string? ExceptionDetail { get; set; }
        public int IsMailSent { get; set; }
        public string? Comment { get; set; }
        public ObjectResult? Response { get; set; }

        public List<SqlParameter> GetSqlParameters()
        {
            List<SqlParameter> paramsList = new List<SqlParameter>();
            paramsList.Add(new SqlParameter("@tx_mail_from", (object?)this.MailFrom ?? DBNull.Value));
            paramsList.Add(new SqlParameter("@jsn_mail_to", (object?)this.MailTo ?? DBNull.Value));
            paramsList.Add(new SqlParameter("@tx_subject", (object?)this.Subject ?? DBNull.Value));
            paramsList.Add(new SqlParameter("@tx_text", (object?)this.Text ?? DBNull.Value));
            paramsList.Add(new SqlParameter("@jsn_mail_cc", (object?)this.MailCc ?? DBNull.Value));
            paramsList.Add(new SqlParameter("@jsn_mail_bcc", (object?)this.MailBcc ?? DBNull.Value));
            paramsList.Add(new SqlParameter("@is_with_files", (object?)this.IsWithFiles ?? DBNull.Value));
            paramsList.Add(new SqlParameter("@is_with_cc", (object?)this.IsWithCc ?? DBNull.Value));
            paramsList.Add(new SqlParameter("@is_with_bcc", (object?)this.IsWithBcc ?? DBNull.Value));
            paramsList.Add(new SqlParameter("@is_exception_occured", (object?)this.IsExceptionOccured ?? DBNull.Value));
            paramsList.Add(new SqlParameter("@tx_exception_status", (object?)this.ExceptionStatus ?? DBNull.Value));
            paramsList.Add(new SqlParameter("@tx_exception_detail", (object?)this.ExceptionDetail ?? DBNull.Value));
            paramsList.Add(new SqlParameter("@is_mail_sent", (object?)this.IsMailSent ?? DBNull.Value));
            paramsList.Add(new SqlParameter("@tx_comment", (object?)this.Comment ?? DBNull.Value));
            return paramsList;
        }

    }
}
