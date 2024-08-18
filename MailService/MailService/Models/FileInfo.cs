namespace MailService.Models
{
    public class FileInfo
    {
        public  string FileName { get; set; }
        public  string Base64 { get; set; }
        public  string Extension { get; set; }
        public FileInfo() { }
        public FileInfo(string FileName, string Base64, string Extension)
        {

            this.FileName = FileName;
            this.Base64 = Base64;
            this.Extension = Extension;

        }
    }
}
