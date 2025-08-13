using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;

namespace SharpAbp.Abp.Crypto.RSA
{
    public interface IRSAEncryptionService
    {
        /// <summary>
        /// Generates an RSA key pair.
        /// </summary>
        /// <param name="keySize"></param>
        /// <param name="rd"></param>
        /// <returns></returns>
        AsymmetricCipherKeyPair GenerateRSAKeyPair(int keySize = 2048, SecureRandom? rd = null);

        /// <summary>
        /// Imports an RSA public key from a PEM formatted string.
        /// </summary>
        /// <param name="publicKeyPem"></param>
        /// <returns></returns>
        AsymmetricKeyParameter ImportPublicKeyPem(string publicKeyPem);

        /// <summary>
        /// Imports an RSA private key from a PEM formatted string.
        /// </summary>
        /// <param name="privateKeyPem"></param>
        /// <returns></returns>
        AsymmetricKeyParameter ImportPrivateKeyPem(string privateKeyPem);

        /// <summary>
        /// Imports an RSA private key from a PEM formatted string (PKCS#8 format).
        /// </summary>
        /// <param name="privateKeyPem"></param>
        /// <returns></returns>
        AsymmetricKeyParameter ImportPrivateKeyPkcs8Pem(string privateKeyPem);

        /// <summary>
        /// Imports an RSA public key from a Base64 encoded DER format string.
        /// </summary>
        /// <param name="publicKeyBase64">The Base64 encoded DER format public key string.</param>
        /// <returns>An <see cref="AsymmetricKeyParameter"/> representing the public key.</returns>
        AsymmetricKeyParameter ImportPublicKey(string publicKeyBase64);

        /// <summary>
        /// Imports an RSA private key from a Base64 encoded DER format string (PKCS#1 format).
        /// </summary>
        /// <param name="privateKeyBase64">The Base64 encoded DER format private key string.</param>
        /// <returns>An <see cref="AsymmetricKeyParameter"/> representing the private key.</returns>
        AsymmetricKeyParameter ImportPrivateKey(string privateKeyBase64);

        /// <summary>
        /// Imports an RSA private key from a Base64 encoded DER format string (PKCS#8 format).
        /// </summary>
        /// <param name="privateKeyBase64">The Base64 encoded DER format private key string.</param>
        /// <returns>An <see cref="AsymmetricKeyParameter"/> representing the private key.</returns>
        AsymmetricKeyParameter ImportPrivateKeyPkcs8(string privateKeyBase64);

        /// <summary>
        ///  RSA encrypts data.
        /// </summary>
        /// <param name="publicKeyParam"></param>
        /// <param name="plainText"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        byte[] Encrypt(AsymmetricKeyParameter publicKeyParam, byte[] plainText, string padding);

        /// <summary>
        /// RSA decrypts data.
        /// </summary>
        /// <param name="privateKeyParam"></param>
        /// <param name="cipherText"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        byte[] Decrypt(AsymmetricKeyParameter privateKeyParam, byte[] cipherText, string padding);

        /// <summary>
        /// RSA signs data.
        /// </summary>
        /// <param name="privateKeyParam"></param>
        /// <param name="data"></param>
        /// <param name="algorithm">SHA1WITHRSA,SHA256WITHRSA,SHA384WITHRSA,SHA512WITHRSA</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        byte[] Sign(AsymmetricKeyParameter privateKeyParam, byte[] data, string algorithm = "SHA256WITHRSA");

        /// <summary>
        /// RSA verifies a signature.
        /// </summary>
        /// <param name="publicKeyParam"></param>
        /// <param name="data"></param>
        /// <param name="signature"></param>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        bool VerifySign(AsymmetricKeyParameter publicKeyParam, byte[] data, byte[] signature, string algorithm = "SHA256WITHRSA");
    }
}
