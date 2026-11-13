using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Builds cache keys for file provider SDK clients. The key is derived from the
    /// parameters that are actually used to construct the client, never from the
    /// container name or other logical identifiers.
    /// </summary>
    public static class FileProviderClientKey
    {
        public static string Build(string provider, params (string Name, object? Value)[] parts)
        {
            Check.NotNullOrWhiteSpace(provider, nameof(provider));

            var builder = new StringBuilder();
            builder.Append("v1|").Append(provider.Length).Append(':').Append(provider);

            if (parts != null)
            {
                foreach (var part in parts)
                {
                    var value = Normalize(part.Value);
                    builder.Append('|')
                        .Append(part.Name.Length).Append(':').Append(part.Name)
                        .Append('=')
                        .Append(value.Length).Append(':').Append(value);
                }
            }

            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(builder.ToString()));

            var hex = new StringBuilder(hash.Length * 2);
            foreach (var value in hash)
            {
                hex.Append(value.ToString("X2"));
            }

            return $"v1|{provider}|{hex}";
        }

        private static string Normalize(object? value)
        {
            return value switch
            {
                null => string.Empty,
                string text => text,
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString() ?? string.Empty
            };
        }
    }
}
