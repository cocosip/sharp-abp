using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities.Encoders;
using System.IO;
using System.Text;

namespace SharpAbp.Abp.Crypto.RSA
{
    /// <summary>
    /// Provides extension methods for <see cref="IRSAEncryptionService"/>.
    /// </summary>
    public static class RSAEncryptionServiceExtensions
    {
        /// <summary>
        /// Encrypts a string using RSA and returns the result as a Base64 string.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="publicKeyParam">The RSA public key parameter.</param>
        /// <param name="plainText">The plain text string to encrypt.</param>
        /// <param name="encoding">The encoding to use for the plain text. Defaults to UTF8.</param>
        /// <param name="padding">The padding scheme to use (e.g., "PKCS1Padding", "OAEPPadding").</param>
        /// <returns>The encrypted data as a Base64 string.</returns>
        public static string Encrypt(this IRSAEncryptionService rsaEncryptionService, AsymmetricKeyParameter publicKeyParam, string plainText, Encoding? encoding = null, string padding = RSAPaddingNames.None)
        {
            encoding ??= Encoding.UTF8;
            var cipherText = rsaEncryptionService.Encrypt(publicKeyParam, encoding.GetBytes(plainText), padding);
            return Base64.ToBase64String(cipherText);
        }

        /// <summary>
        /// Encrypts a string using RSA with a public key PEM string and returns the result as a Base64 string.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="publicKeyPem">The PEM formatted string containing the public key.</param>
        /// <param name="plainText">The plain text string to encrypt.</param>
        /// <param name="encoding">The encoding to use for the plain text. Defaults to UTF8.</param>
        /// <param name="padding">The padding scheme to use (e.g., "PKCS1Padding", "OAEPPadding").</param>
        /// <returns>The encrypted data as a Base64 string.</returns>
        public static string EncryptFromPem(this IRSAEncryptionService rsaEncryptionService, string publicKeyPem, string plainText, Encoding? encoding = null, string padding = RSAPaddingNames.None)
        {
            var publicKeyParam = rsaEncryptionService.ImportPublicKeyPem(publicKeyPem);
            return rsaEncryptionService.Encrypt(publicKeyParam, plainText, encoding, padding);
        }

        /// <summary>
        /// Decrypts a Base64 encoded string using RSA and returns the result as a string.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="privateKeyParam">The RSA private key parameter.</param>
        /// <param name="cipherText">The Base64 encoded encrypted data.</param>
        /// <param name="encoding">The encoding to use for the decrypted text. Defaults to UTF8.</param>
        /// <param name="padding">The padding scheme used during encryption (e.g., "PKCS1Padding", "OAEPPadding").</param>
        /// <returns>The decrypted data as a string.</returns>
        public static string Decrypt(this IRSAEncryptionService rsaEncryptionService, AsymmetricKeyParameter privateKeyParam, string cipherText, Encoding? encoding = null, string padding = RSAPaddingNames.None)
        {
            encoding ??= Encoding.UTF8;
            var cipherBytes = Base64.Decode(cipherText);
            var plainText = rsaEncryptionService.Decrypt(privateKeyParam, cipherBytes, padding);
            return encoding.GetString(plainText);
        }

        /// <summary>
        /// Decrypts a Base64 encoded string using RSA with a private key PEM string and returns the result as a string.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="privateKeyPem">The PEM formatted string containing the private key.</param>
        /// <param name="cipherText">The Base64 encoded encrypted data.</param>
        /// <param name="encoding">The encoding to use for the decrypted text. Defaults to UTF8.</param>
        /// <param name="padding">The padding scheme used during encryption (e.g., "PKCS1Padding", "OAEPPadding").</param>
        /// <returns>The decrypted data as a string.</returns>
        public static string DecryptFromPem(this IRSAEncryptionService rsaEncryptionService, string privateKeyPem, string cipherText, Encoding? encoding = null, string padding = RSAPaddingNames.None)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKeyPem(privateKeyPem);
            return rsaEncryptionService.Decrypt(privateKeyParam, cipherText, encoding, padding);
        }

        /// <summary>
        /// Decrypts a Base64 encoded string using RSA with a PKCS#8 private key PEM string and returns the result as a string.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="privateKeyPem">The PEM formatted string containing the PKCS#8 private key.</param>
        /// <param name="cipherText">The Base64 encoded encrypted data.</param>
        /// <param name="encoding">The encoding to use for the decrypted text. Defaults to UTF8.</param>
        /// <param name="padding">The padding scheme used during encryption (e.g., "PKCS1Padding", "OAEPPadding").</param>
        /// <returns>The decrypted data as a string.</returns>
        public static string DecryptFromPkcs8Pem(this IRSAEncryptionService rsaEncryptionService, string privateKeyPem, string cipherText, Encoding? encoding = null, string padding = RSAPaddingNames.None)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKeyPkcs8Pem(privateKeyPem);
            return rsaEncryptionService.Decrypt(privateKeyParam, cipherText, encoding, padding);
        }

        /// <summary>
        /// Signs a string using RSA and returns the signature as a Base64 string.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="privateKeyParam">The RSA private key parameter.</param>
        /// <param name="data">The data string to sign.</param>
        /// <param name="algorithm">The signing algorithm (e.g., "SHA256WITHRSA", "SHA1WITHRSA"). Default is "SHA256WITHRSA".</param>
        /// <param name="encoding">The encoding to use for the data. Defaults to UTF8.</param>
        /// <returns>The signature as a Base64 string.</returns>
        public static string Sign(this IRSAEncryptionService rsaEncryptionService, AsymmetricKeyParameter privateKeyParam, string data, string algorithm = "SHA256WITHRSA", Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var d = encoding.GetBytes(data);
            var signBuffer = rsaEncryptionService.Sign(privateKeyParam, d, algorithm);
            return Base64.ToBase64String(signBuffer);
        }

        /// <summary>
        /// Signs a string using RSA with a private key PEM string and returns the signature as a Base64 string.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="privateKeyPem">The PEM formatted string containing the private key.</param>
        /// <param name="data">The data string to sign.</param>
        /// <param name="algorithm">The signing algorithm (e.g., "SHA256WITHRSA", "SHA1WITHRSA"). Default is "SHA256WITHRSA".</param>
        /// <param name="encoding">The encoding to use for the data. Defaults to UTF8.</param>
        /// <returns>The signature as a Base64 string.</returns>
        public static string SignFromPem(this IRSAEncryptionService rsaEncryptionService, string privateKeyPem, string data, string algorithm = "SHA256WITHRSA", Encoding? encoding = null)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKeyPem(privateKeyPem);
            return rsaEncryptionService.Sign(privateKeyParam, data, algorithm, encoding);
        }

        /// <summary>
        /// Signs a string using RSA with a PKCS#8 private key PEM string and returns the signature as a Base64 string.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="privateKeyPem">The PEM formatted string containing the PKCS#8 private key.</param>
        /// <param name="data">The data string to sign.</param>
        /// <param name="algorithm">The signing algorithm (e.g., "SHA256WITHRSA", "SHA1WITHRSA"). Default is "SHA256WITHRSA".</param>
        /// <param name="encoding">The encoding to use for the data. Defaults to UTF8.</param>
        /// <returns>The signature as a Base64 string.</returns>
        public static string SignFromPkcs8Pem(this IRSAEncryptionService rsaEncryptionService, string privateKeyPem, string data, string algorithm = "SHA256WITHRSA", Encoding? encoding = null)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKeyPkcs8Pem(privateKeyPem);
            return rsaEncryptionService.Sign(privateKeyParam, data, algorithm, encoding);
        }

        /// <summary>
        /// Verifies a Base64 encoded signature using RSA.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="publicKeyParam">The RSA public key parameter.</param>
        /// <param name="data">The original data string that was signed.</param>
        /// <param name="signature">The Base64 encoded signature to verify.</param>
        /// <param name="algorithm">The signing algorithm used (e.g., "SHA256WITHRSA", "SHA1WITHRSA"). Default is "SHA256WITHRSA".</param>
        /// <param name="encoding">The encoding to use for the data. Defaults to UTF8.</param>
        /// <returns>True if the signature is valid, false otherwise.</returns>
        public static bool VerifySign(this IRSAEncryptionService rsaEncryptionService, AsymmetricKeyParameter publicKeyParam, string data, string signature, string algorithm = "SHA256WITHRSA", Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var signatureBuffer = Base64.Decode(signature);
            var dataBuffer = encoding.GetBytes(data);
            return rsaEncryptionService.VerifySign(publicKeyParam, dataBuffer, signatureBuffer, algorithm);
        }

        /// <summary>
        /// Verifies a Base64 encoded signature using RSA with a public key PEM string.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="publicKeyPem">The PEM formatted string containing the public key.</param>
        /// <param name="data">The original data string that was signed.</param>
        /// <param name="signature">The Base64 encoded signature to verify.</param>
        /// <param name="algorithm">The signing algorithm used (e.g., "SHA256WITHRSA", "SHA1WITHRSA"). Default is "SHA256WITHRSA".</param>
        /// <param name="encoding">The encoding to use for the data. Defaults to UTF8.</param>
        /// <returns>True if the signature is valid, false otherwise.</returns>
        public static bool VerifySignFromPem(this IRSAEncryptionService rsaEncryptionService, string publicKeyPem, string data, string signature, string algorithm = "SHA256WITHRSA", Encoding? encoding = null)
        {
            var publicKeyParam = rsaEncryptionService.ImportPublicKeyPem(publicKeyPem);
            return rsaEncryptionService.VerifySign(publicKeyParam, data, signature, algorithm, encoding);
        }

        /// <summary>
        /// Encrypts a string using RSA with a Base64 encoded public key (DER format) and returns the result as a Base64 string.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="publicKeyBase64">The Base64 encoded string containing the public key in DER format.</param>
        /// <param name="plainText">The plain text string to encrypt.</param>
        /// <param name="encoding">The encoding to use for the plain text. Defaults to UTF8.</param>
        /// <param name="padding">The padding scheme to use (e.g., "PKCS1Padding", "OAEPPadding").</param>
        /// <returns>The encrypted data as a Base64 string.</returns>
        public static string EncryptFromBase64(this IRSAEncryptionService rsaEncryptionService, string publicKeyBase64, string plainText, Encoding? encoding = null, string padding = RSAPaddingNames.None)
        {
            var publicKeyParam = rsaEncryptionService.ImportPublicKey(publicKeyBase64);
            return rsaEncryptionService.Encrypt(publicKeyParam, plainText, encoding, padding);
        }

        /// <summary>
        /// Decrypts a Base64 encoded string using RSA with a Base64 encoded private key (PKCS#1 DER format) and returns the result as a string.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="privateKeyBase64">The Base64 encoded string containing the private key in PKCS#1 DER format.</param>
        /// <param name="cipherText">The Base64 encoded encrypted data.</param>
        /// <param name="encoding">The encoding to use for the decrypted text. Defaults to UTF8.</param>
        /// <param name="padding">The padding scheme used during encryption (e.g., "PKCS1Padding", "OAEPPadding").</param>
        /// <returns>The decrypted data as a string.</returns>
        public static string DecryptFromBase64(this IRSAEncryptionService rsaEncryptionService, string privateKeyBase64, string cipherText, Encoding? encoding = null, string padding = RSAPaddingNames.None)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKey(privateKeyBase64);
            return rsaEncryptionService.Decrypt(privateKeyParam, cipherText, encoding, padding);
        }

        /// <summary>
        /// Decrypts a Base64 encoded string using RSA with a Base64 encoded private key (PKCS#8 DER format) and returns the result as a string.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="privateKeyBase64">The Base64 encoded string containing the private key in PKCS#8 DER format.</param>
        /// <param name="cipherText">The Base64 encoded encrypted data.</param>
        /// <param name="encoding">The encoding to use for the decrypted text. Defaults to UTF8.</param>
        /// <param name="padding">The padding scheme used during encryption (e.g., "PKCS1Padding", "OAEPPadding").</param>
        /// <returns>The decrypted data as a string.</returns>
        public static string DecryptFromPkcs8Base64(this IRSAEncryptionService rsaEncryptionService, string privateKeyBase64, string cipherText, Encoding? encoding = null, string padding = RSAPaddingNames.None)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKeyPkcs8(privateKeyBase64);
            return rsaEncryptionService.Decrypt(privateKeyParam, cipherText, encoding, padding);
        }

        /// <summary>
        /// Signs a string using RSA with a Base64 encoded private key (PKCS#1 DER format) and returns the signature as a Base64 string.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="privateKeyBase64">The Base64 encoded string containing the private key in PKCS#1 DER format.</param>
        /// <param name="data">The data string to sign.</param>
        /// <param name="algorithm">The signing algorithm (e.g., "SHA256WITHRSA", "SHA1WITHRSA"). Default is "SHA256WITHRSA".</param>
        /// <param name="encoding">The encoding to use for the data. Defaults to UTF8.</param>
        /// <returns>The signature as a Base64 string.</returns>
        public static string SignFromBase64(this IRSAEncryptionService rsaEncryptionService, string privateKeyBase64, string data, string algorithm = "SHA256WITHRSA", Encoding? encoding = null)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKey(privateKeyBase64);
            return rsaEncryptionService.Sign(privateKeyParam, data, algorithm, encoding);
        }

        /// <summary>
        /// Signs a string using RSA with a Base64 encoded private key (PKCS#8 DER format) and returns the signature as a Base64 string.
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="privateKeyBase64">The Base64 encoded string containing the private key in PKCS#8 DER format.</param>
        /// <param name="data">The data string to sign.</param>
        /// <param name="algorithm">The signing algorithm (e.g., "SHA256WITHRSA", "SHA1WITHRSA"). Default is "SHA256WITHRSA".</param>
        /// <param name="encoding">The encoding to use for the data. Defaults to UTF8.</param>
        /// <returns>The signature as a Base64 string.</returns>
        public static string SignFromPkcs8Base64(this IRSAEncryptionService rsaEncryptionService, string privateKeyBase64, string data, string algorithm = "SHA256WITHRSA", Encoding? encoding = null)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKeyPkcs8(privateKeyBase64);
            return rsaEncryptionService.Sign(privateKeyParam, data, algorithm, encoding);
        }

        /// <summary>
        /// Verifies a Base64 encoded signature using RSA with a Base64 encoded public key (DER format).
        /// </summary>
        /// <param name="rsaEncryptionService">The RSA encryption service instance.</param>
        /// <param name="publicKeyBase64">The Base64 encoded string containing the public key in DER format.</param>
        /// <param name="data">The original data string that was signed.</param>
        /// <param name="signature">The Base64 encoded signature to verify.</param>
        /// <param name="algorithm">The signing algorithm used (e.g., "SHA256WITHRSA", "SHA1WITHRSA"). Default is "SHA256WITHRSA".</param>
        /// <param name="encoding">The encoding to use for the data. Defaults to UTF8.</param>
        /// <returns>True if the signature is valid, false otherwise.</returns>
        public static bool VerifySignFromBase64(this IRSAEncryptionService rsaEncryptionService, string publicKeyBase64, string data, string signature, string algorithm = "SHA256WITHRSA", Encoding? encoding = null)
        {
            var publicKeyParam = rsaEncryptionService.ImportPublicKey(publicKeyBase64);
            return rsaEncryptionService.VerifySign(publicKeyParam, data, signature, algorithm, encoding);
        }
    }
}
