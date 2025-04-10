using Microsoft.Extensions.Configuration;
using System.Text;

namespace CDFHEXRETE.Common.Extensions
{
    public static class CustomConfigProviderExtensions
    {
        public static IConfigurationBuilder AddEncryptConfProvider(this IConfigurationBuilder builder
        , string path, string key, string ivKey, bool reloadOnChange)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (!File.Exists(path))
                throw new FileNotFoundException($"{path} not found");

            var source = new EncryptJsonConfSource
            {
                FileProvider = null,
                Optional = false,
                Path = path,
                ReloadOnChange = reloadOnChange,
                //Key = key,
                //IV = ivKey
                Key = Convert.ToHexString(Convert.FromBase64String(key)),
                IV = Convert.ToHexString(Convert.FromBase64String(ivKey))
            };

            return builder.Add(source);
        }
}
}
