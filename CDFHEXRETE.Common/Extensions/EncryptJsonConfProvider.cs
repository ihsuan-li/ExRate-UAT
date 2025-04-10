using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CDFHEXRETE.Common.Extensions
{
    public class EncryptJsonConfProvider : JsonConfigurationProvider
    {

        public const string ENCRYPT_PREFIX = "ENC";

        public string Key { get; set; } = String.Empty;
        public string IV { get; set; } = String.Empty;

        private String path = String.Empty;

        public EncryptJsonConfProvider(JsonConfigurationSource source) : base(source)
        {
        }

        public override void Load()
        {
            base.Load();
            using (var aes = Aes.Create())
            {
                //    aes.Key = Encoding.UTF8.GetBytes(Key);
                //    aes.IV = Encoding.UTF8.GetBytes(IV);
                aes.Key = Convert.FromHexString(Key);
                aes.IV = Convert.FromHexString(IV);
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                foreach (var item in Data)
                {
                    if (item.Value.StartsWith(ENCRYPT_PREFIX))
                    {
                        var encryptedText = item.Value.Substring(4, item.Value.Length - 5);
                        encryptedText = Convert.ToHexString(Convert.FromBase64String(encryptedText));

                        //using (var ms = new MemoryStream(Convert.FromBase64String(encryptedText)))
                        using (var ms = new MemoryStream(Convert.FromHexString(encryptedText)))
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        using (var sr = new StreamReader(cs))
                        {
                            var decrypted = sr.ReadToEnd();
                            Data[item.Key] = decrypted;
                        }
                    }
                }
            }
        }

    }

    public class EncryptJsonConfSource : JsonConfigurationSource
    {
        public string Key { get; set; } = String.Empty;
        public string IV { get; set; } = String.Empty;

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new EncryptJsonConfProvider(this)
            {
                Key = Key,
                IV = IV
            };
        }
    }
}
