using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace MailService.Models
{
    public class MailSetting
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Name { get; set; }
        public string? MailFrom { get; set; }
        public string? Password { get; set; }  
        
        public MailSetting getCredentials(string v1, string v2) {
            var encryptionHelper = new EncryptionHelper(v1,v2);
            return new MailSetting() { 
                Host = encryptionHelper.Decrypt(this.Host),
                Port = this.Port,
                Name = encryptionHelper.Decrypt(this.Name),
                MailFrom = encryptionHelper.Decrypt(this.MailFrom),
                Password = encryptionHelper.Decrypt(this.Password)
            };
        }
    }
}
