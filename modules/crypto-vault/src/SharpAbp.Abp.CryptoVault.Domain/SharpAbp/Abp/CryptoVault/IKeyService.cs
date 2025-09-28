using System.Collections.Generic;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Provides cryptographic key management services including RSA and SM2 key generation, encryption, and decryption.
    /// This service handles secure key storage, credential management, and cryptographic operations.
    /// </summary>
    public interface IKeyService
    {
        /// <summary>
        /// Encrypts a plain text key using the specified passphrase and salt.
        /// </summary>
        /// <param name="plainText">The plain text key to encrypt.</param>
        /// <param name="passPhrase">The passphrase used for encryption.</param>
        /// <param name="salt">The salt value used to strengthen encryption.</param>
        /// <returns>The encrypted key as a string.</returns>
        string EncryptKey(string plainText, string passPhrase, string salt);

        /// <summary>
        /// Decrypts an encrypted key using the specified passphrase and salt.
        /// </summary>
        /// <param name="cipherText">The encrypted key to decrypt.</param>
        /// <param name="passPhrase">The passphrase used for decryption.</param>
        /// <param name="salt">The salt value used during encryption.</param>
        /// <returns>The decrypted plain text key.</returns>
        string DecryptKey(string cipherText, string passPhrase, string salt);

        /// <summary>
        /// Generates a specified number of RSA credential pairs with the given key size.
        /// Each credential includes encrypted public and private keys with associated metadata.
        /// </summary>
        /// <param name="size">The RSA key size in bits (e.g., 1024, 2048, 4096).</param>
        /// <param name="count">The number of RSA credential pairs to generate.</param>
        /// <returns>A list of generated RSA credentials with encrypted keys.</returns>
        List<RSACreds> GenerateRSACreds(int size, int count);

        /// <summary>
        /// Generates a specified number of SM2 credential pairs using the given elliptic curve.
        /// Each credential includes encrypted public and private keys with associated metadata.
        /// </summary>
        /// <param name="curve">The elliptic curve name for SM2 (e.g., sm2p256v1, wapip192v1).</param>
        /// <param name="count">The number of SM2 credential pairs to generate.</param>
        /// <returns>A list of generated SM2 credentials with encrypted keys.</returns>
        List<SM2Creds> GenerateSM2Creds(string curve, int count);

        /// <summary>
        /// Generates a unique identifier for cryptographic credentials.
        /// </summary>
        /// <returns>A unique identifier string.</returns>
        string GenerateIdentifier();

        /// <summary>
        /// Generates a secure passphrase for key encryption.
        /// </summary>
        /// <returns>A randomly generated passphrase string.</returns>
        string GeneratePassPhrase();

        /// <summary>
        /// Generates a salt value for key encryption strengthening.
        /// </summary>
        /// <returns>A randomly generated salt string.</returns>
        string GenerateSalt();
    }
}
