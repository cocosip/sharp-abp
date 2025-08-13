using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SharpAbp.Abp.Crypto.SM2
{
    public interface ISm2EncryptionService
    {
        /// <summary>
        /// Generates an SM2 key pair.
        /// </summary>
        /// <param name="curve">The curve name to use, defaults to Sm2p256v1.</param>
        /// <param name="rd">Optional: A secure random number generator. If null, a new one will be created.</param>
        /// <returns>An <see cref="AsymmetricCipherKeyPair"/> containing the public and private keys.</returns>
        AsymmetricCipherKeyPair GenerateSm2KeyPair(string curve = Sm2EncryptionNames.CurveSm2p256v1, SecureRandom? rd = null);

        /// <summary>
        /// Encrypts data using the SM2 public key.
        /// </summary>
        /// <param name="publicKey">The public key as a byte array.</param>
        /// <param name="plainText">The plain text data to encrypt.</param>
        /// <param name="curve">The curve name used for encryption. Defaults to Sm2p256v1.</param>
        /// <param name="mode">The encryption mode. Defaults to C1C2C3.</param>
        /// <returns>The encrypted data as a byte array.</returns>
        byte[] Encrypt(byte[] publicKey, byte[] plainText, string curve = Sm2EncryptionNames.CurveSm2p256v1, Mode mode = Mode.C1C2C3);

        /// <summary>
        /// Decrypts data using the SM2 private key.
        /// </summary>
        /// <param name="privateKey">The private key as a byte array.</param>
        /// <param name="cipherText">The encrypted data to decrypt.</param>
        /// <param name="curve">The curve name used for decryption. Defaults to Sm2p256v1.</param>
        /// <param name="mode">The encryption mode. Defaults to C1C2C3.</param>
        /// <returns>The decrypted data as a byte array.</returns>
        byte[] Decrypt(byte[] privateKey, byte[] cipherText, string curve = Sm2EncryptionNames.CurveSm2p256v1, Mode mode = Mode.C1C2C3);

        /// <summary>
        /// Signs data using the SM2 private key.
        /// </summary>
        /// <param name="privateKey">The private key as a byte array.</param>
        /// <param name="plainText">The plain text data to sign.</param>
        /// <param name="curve">The curve name used for signing. Defaults to Sm2p256v1.</param>
        /// <param name="id">Optional: The ID to use for signing. If null, a default ID will be used.</param>
        /// <returns>The signature as a byte array.</returns>
        byte[] Sign(byte[] privateKey, byte[] plainText, string curve = Sm2EncryptionNames.CurveSm2p256v1, byte[]? id = null);

        /// <summary>
        /// Verifies a signature using the SM2 public key.
        /// </summary>
        /// <param name="publicKey">The public key as a byte array.</param>
        /// <param name="plainText">The original plain text data.</param>
        /// <param name="signature">The signature to verify.</param>
        /// <param name="curve">The curve name used for verification. Defaults to Sm2p256v1.</param>
        /// <param name="id">Optional: The ID used during signing. If null, a default ID will be used.</param>
        /// <returns>True if the signature is valid, false otherwise.</returns>
        bool VerifySign(byte[] publicKey, byte[] plainText, byte[] signature, string curve = Sm2EncryptionNames.CurveSm2p256v1, byte[]? id = null);

        /// <summary>
        /// Converts C1C2C3 encoded ciphertext to C1C3C2 encoded ciphertext.
        /// </summary>
        /// <param name="c1c2c3">The C1C2C3 encoded ciphertext.</param>
        /// <param name="curve">The curve name used for encryption. Defaults to Sm2p256v1.</param>
        /// <returns>The C1C3C2 encoded ciphertext.</returns>
        byte[] C123ToC132(byte[] c1c2c3, string curve = Sm2EncryptionNames.CurveSm2p256v1);

        /// <summary>
        /// Converts C1C3C2 encoded ciphertext to C1C2C3 encoded ciphertext.
        /// </summary>
        /// <param name="c1c3c2">The C1C3C2 encoded ciphertext.</param>
        /// <param name="curve">The curve name used for encryption. Defaults to Sm2p256v1.</param>
        /// <returns>The C1C2C3 encoded ciphertext.</returns>
        byte[] C132ToC123(byte[] c1c3c2, string curve = Sm2EncryptionNames.CurveSm2p256v1);
    }
}
