using Org.BouncyCastle.Utilities.Encoders;
using System.Text;

namespace SharpAbp.Abp.Crypto.SM3
{
    /// <summary>
    /// Provides extension methods for <see cref="ISm3EncryptionService"/>.
    /// </summary>
    public static class Sm3EncryptionServiceExtensions
    {
        /// <summary>
        /// Computes the SM3 hash of a string and returns the result as a hexadecimal string.
        /// </summary>
        /// <param name="sm3EncryptionService">The SM3 encryption service instance.</param>
        /// <param name="plainText">The plain text string to hash.</param>
        /// <param name="encoding">The encoding to use for the plain text. Defaults to UTF8.</param>
        /// <returns>The SM3 hash as a hexadecimal string.</returns>
        public static string GetHash(
            this ISm3EncryptionService sm3EncryptionService,
            string plainText,
            Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var buffer = encoding.GetBytes(plainText);
            var hashBytes = sm3EncryptionService.GetHash(buffer);
            return Hex.ToHexString(hashBytes);
        }
    }
}