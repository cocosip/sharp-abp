using Org.BouncyCastle.Utilities.Encoders;
using System.Text;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SharpAbp.Abp.Crypto.SM2
{
    /// <summary>
    /// Provides extension methods for <see cref="ISm2EncryptionService"/>.
    /// </summary>
    public static class Sm2EncryptionServiceExtensions
    {
        /// <summary>
        /// Encrypts a string using the SM2 public key and returns the result as a hexadecimal string.
        /// </summary>
        /// <param name="sm2EncryptionService">The SM2 encryption service instance.</param>
        /// <param name="publicKey">The public key as a hexadecimal string.</param>
        /// <param name="plainText">The plain text string to encrypt.</param>
        /// <param name="encoding">The encoding to use for the plain text. Defaults to UTF8.</param>
        /// <param name="curve">The curve name used for encryption. Defaults to Sm2p256v1.</param>
        /// <param name="mode">The encryption mode. Defaults to C1C2C3.</param>
        /// <returns>The encrypted data as a hexadecimal string.</returns>
        public static string Encrypt(
            this ISm2EncryptionService sm2EncryptionService,
            string publicKey,
            string plainText,
            Encoding? encoding = null,
            string curve = Sm2EncryptionNames.CurveSm2p256v1,
            Mode mode = Mode.C1C2C3)
        {
            encoding ??= Encoding.UTF8;
            var pub = Hex.DecodeStrict(publicKey.ToLower());
            var buffer = encoding.GetBytes(plainText);
            var v = sm2EncryptionService.Encrypt(pub, buffer, curve, mode);
            return Hex.ToHexString(v);
        }

        /// <summary>
        /// Decrypts a hexadecimal string using the SM2 private key and returns the result as a string.
        /// </summary>
        /// <param name="sm2EncryptionService">The SM2 encryption service instance.</param>
        /// <param name="privateKey">The private key as a hexadecimal string.</param>
        /// <param name="cipherText">The hexadecimal encoded encrypted data.</param>
        /// <param name="encoding">The encoding to use for the decrypted text. Defaults to UTF8.</param>
        /// <param name="curve">The curve name used for decryption. Defaults to Sm2p256v1.</param>
        /// <param name="mode">The encryption mode. Defaults to C1C2C3.</param>
        /// <returns>The decrypted data as a string.</returns>
        public static string Decrypt(
            this ISm2EncryptionService sm2EncryptionService,
            string privateKey,
            string cipherText,
            Encoding? encoding = null,
            string curve = Sm2EncryptionNames.CurveSm2p256v1,
            Mode mode = Mode.C1C2C3)
        {
            encoding ??= Encoding.UTF8;
            var aPrivy = Hex.DecodeStrict(privateKey.ToLower());
            var buffer = Hex.DecodeStrict(cipherText);
            var v = sm2EncryptionService.Decrypt(aPrivy, buffer, curve, mode);
            return encoding.GetString(v);
        }

        /// <summary>
        /// Signs a string using the SM2 private key and returns the signature as a hexadecimal string.
        /// </summary>
        /// <param name="sm2EncryptionService">The SM2 encryption service instance.</param>
        /// <param name="privateKey">The private key as a hexadecimal string.</param>
        /// <param name="plainText">The plain text data to sign.</param>
        /// <param name="encoding">The encoding to use for the plain text. Defaults to UTF8.</param>
        /// <param name="curve">The curve name used for signing. Defaults to Sm2p256v1.</param>
        /// <param name="id">Optional: The ID to use for signing. If null, a default ID will be used.</param>
        /// <returns>The signature as a hexadecimal string.</returns>
        public static string Sign(
            this ISm2EncryptionService sm2EncryptionService,
            string privateKey,
            string plainText,
            Encoding? encoding = null,
            string curve = Sm2EncryptionNames.CurveSm2p256v1,
            byte[]? id = null)
        {
            encoding ??= Encoding.UTF8;
            var aPrivy = Hex.DecodeStrict(privateKey.ToLower());
            var buffer = encoding.GetBytes(plainText);
            var v = sm2EncryptionService.Sign(aPrivy, buffer, curve, id);
            return Hex.ToHexString(v);
        }

        /// <summary>
        /// Verifies a hexadecimal encoded signature using the SM2 public key.
        /// </summary>
        /// <param name="sm2EncryptionService">The SM2 encryption service instance.</param>
        /// <param name="publicKey">The public key as a hexadecimal string.</param>
        /// <param name="plainText">The original plain text data.</param>
        /// <param name="signature">The hexadecimal encoded signature to verify.</param>
        /// <param name="encoding">The encoding to use for the plain text. Defaults to UTF8.</param>
        /// <param name="curve">The curve name used for verification. Defaults to Sm2p256v1.</param>
        /// <param name="id">Optional: The ID used during signing. If null, a default ID will be used.</param>
        /// <returns>True if the signature is valid, false otherwise.</returns>
        public static bool VerifySign(
            this ISm2EncryptionService sm2EncryptionService,
            string publicKey,
            string plainText,
            string signature,
            Encoding? encoding = null,
            string curve = Sm2EncryptionNames.CurveSm2p256v1,
            byte[]? id = null)
        {
            encoding ??= Encoding.UTF8;
            var aPub = Hex.DecodeStrict(publicKey);
            var s = Hex.DecodeStrict(signature);
            var buffer = encoding.GetBytes(plainText);
            return sm2EncryptionService.VerifySign(aPub, buffer, s, curve, id);
        }
    }
}
