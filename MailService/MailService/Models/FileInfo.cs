namespace MailService.Models
{
    public class FileInfo
    {
        public required string FileName { get; set; }
        public required string Base64 { get; set; }
        public required string Extension { get; set; }
    }
}
