namespace MailService.Models
{
    public class MailBody
 
    {
        public bool IsWithAttachment { get; set; }
        public bool IsWithCc { get; set; }
        public bool IsWithBcc { get; set; }
        public Dictionary<String, String>? ToMail { get; set; }
        public string? Subject { get; set; }
        public string? Text { get; set; }
        public Dictionary<String, String>? CcEmail { get; set; }
        public Dictionary<String, String>? BccEmail { get; set; }
        public List<FileInfo>? Files { get; set; }
    }
}
