namespace MailService.Models
{
    public class MailSetting
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Name { get; set; }
        public string? MailFrom { get; set; }
        public string? Password { get; set; }
    }
}
