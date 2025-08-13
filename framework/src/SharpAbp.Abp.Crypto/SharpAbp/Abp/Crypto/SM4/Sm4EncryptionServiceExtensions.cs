using Org.BouncyCastle.Utilities.Encoders;
using System.Text;

namespace SharpAbp.Abp.Crypto.SM4
{
    /// <summary>
    /// Provides extension methods for <see cref="ISm4EncryptionService"/>.
    /// </summary>
    public static class Sm4EncryptionServiceExtensions
    {
        /// <summary>
        /// Encrypts a string using SM4 and returns the result as a hexadecimal string.
        /// </summary>
        /// <param name="sm4EncryptionService">The SM4 encryption service instance.</param>
        /// <param name="plainText">The plain text string to encrypt.</param>
        /// <param name="keyHex">The encryption key as a hexadecimal string (16 bytes).</param>
        /// <param name="ivHex">The Initialization Vector (IV) as a hexadecimal string (16 bytes for CBC mode).</param>
        /// <param name="mode">The encryption mode (e.g., ECB, CBC). Defaults to the configured default mode.</param>
        /// <param name="padding">The padding scheme (e.g., PKCS7Padding, NoPadding). Defaults to the configured default padding.</param>
        /// <param name="encoding">The encoding to use for the plain text. Defaults to UTF8.</param>
        /// <returns>The encrypted data as a hexadecimal string.</returns>
        public static string Encrypt(
            this ISm4EncryptionService sm4EncryptionService,
            string plainText,
            string keyHex,
            string ivHex,
            string mode = "",
            string padding = "",
            Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var plainTextBytes = encoding.GetBytes(plainText);
            var keyBytes = Hex.DecodeStrict(keyHex);
            var ivBytes = Hex.DecodeStrict(ivHex);

            var cipherTextBytes = sm4EncryptionService.Encrypt(
                plainTextBytes,
                keyBytes,
                ivBytes,
                mode,
                padding);

            return Hex.ToHexString(cipherTextBytes);
        }

        /// <summary>
        /// Decrypts a hexadecimal string using SM4 and returns the result as a string.
        /// </summary>
        /// <param name="sm4EncryptionService">The SM4 encryption service instance.</param>
        /// <param name="cipherTextHex">The encrypted data as a hexadecimal string.</param>
        /// <param name="keyHex">The decryption key as a hexadecimal string (16 bytes).</param>
        /// <param name="ivHex">The Initialization Vector (IV) as a hexadecimal string (16 bytes for CBC mode).</param>
        /// <param name="mode">The decryption mode (e.g., ECB, CBC). Defaults to the configured default mode.</param>
        /// <param name="padding">The padding scheme (e.g., PKCS7Padding, NoPadding). Defaults to the configured default padding.</param>
        /// <param name="encoding">The encoding to use for the decrypted text. Defaults to UTF8.</param>
        /// <returns>The decrypted data as a string.</returns>
        public static string Decrypt(
            this ISm4EncryptionService sm4EncryptionService,
            string cipherTextHex,
            string keyHex,
            string ivHex,
            string mode = "",
            string padding = "",
            Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var cipherTextBytes = Hex.DecodeStrict(cipherTextHex);
            var keyBytes = Hex.DecodeStrict(keyHex);
            var ivBytes = Hex.DecodeStrict(ivHex);

            var plainTextBytes = sm4EncryptionService.Decrypt(
                cipherTextBytes,
                keyBytes,
                ivBytes,
                mode,
                padding);

            return encoding.GetString(plainTextBytes);
        }
    }
}
