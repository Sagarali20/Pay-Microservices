using Common;

namespace MailService.Models
{
    public class RabbitMQSetting
    {
        public string? Host { get; set; }
        public string? Queue { get; set; }
        public int Port { get; set; }
        public RabbitMQSetting getCredentials(string v1, string v2)
        {
            var encryptionHelper = new EncryptionHelper(v1, v2);
            return new RabbitMQSetting()
            {
                Host = encryptionHelper.Decrypt(this.Host),
                Queue = encryptionHelper.Decrypt(this.Queue),
                Port = this.Port
            };
        }
    }
}
