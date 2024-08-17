using System.Security.Cryptography;
using System.Text;

namespace PaymentService.Helpers
{
    public class EncryptionHelper
    {
        private readonly byte[] Key; //encryption key
        private readonly byte[] IV; //initialization vector (IV)
        public EncryptionHelper(string key, string iv)
        {
            Key = Encoding.UTF8.GetBytes(key);
            IV = Encoding.UTF8.GetBytes(iv);
        }
        public string Decrypt(string cipherText)
        {
            byte[] bytes = Convert.FromBase64String(cipherText);
            using (Aes aesAlg = Aes.Create())
            {

                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
