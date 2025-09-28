using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.Crypto.SM2;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Security.Encryption;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Default implementation of <see cref="IKeyService"/> that provides cryptographic key management services.
    /// This service handles RSA and SM2 key generation, encryption/decryption operations, and secure credential storage.
    /// Registered as a transient dependency to ensure thread safety and proper resource management.
    /// </summary>
    public class DefaultKeyService : IKeyService, ITransientDependency
    {
        /// <summary>
        /// Gets the GUID generator service for creating unique identifiers.
        /// </summary>
        protected IGuidGenerator GuidGenerator { get; }

        /// <summary>
        /// Gets the string encryption service for encrypting/decrypting key data.
        /// </summary>
        protected IStringEncryptionService StringEncryptionService { get; }

        /// <summary>
        /// Gets the RSA encryption service for RSA key pair generation and operations.
        /// </summary>
        protected IRSAEncryptionService RSAEncryptionService { get; }

        /// <summary>
        /// Gets the SM2 encryption service for SM2 key pair generation and operations.
        /// </summary>
        protected ISm2EncryptionService Sm2EncryptionService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultKeyService"/> class.
        /// </summary>
        /// <param name="guidGenerator">The GUID generator service.</param>
        /// <param name="stringEncryptionService">The string encryption service.</param>
        /// <param name="rsaEncryptionService">The RSA encryption service.</param>
        /// <param name="sm2EncryptionService">The SM2 encryption service.</param>
        public DefaultKeyService(
            IGuidGenerator guidGenerator,
            IStringEncryptionService stringEncryptionService,
            IRSAEncryptionService rsaEncryptionService,
            ISm2EncryptionService sm2EncryptionService)
        {
            GuidGenerator = guidGenerator;
            StringEncryptionService = stringEncryptionService;
            RSAEncryptionService = rsaEncryptionService;
            Sm2EncryptionService = sm2EncryptionService;
        }

        /// <summary>
        /// Generates a specified number of RSA credential pairs with the given key size.
        /// Each credential includes encrypted public and private keys with unique identifiers and secure passphrases.
        /// </summary>
        /// <param name="size">The RSA key size in bits (e.g., 1024, 2048, 4096).</param>
        /// <param name="count">The number of RSA credential pairs to generate.</param>
        /// <returns>A list of generated RSA credentials with encrypted keys and metadata.</returns>
        public virtual List<RSACreds> GenerateRSACreds(int size, int count)
        {
            var rsaCredsList = new List<RSACreds>();
            
            for (int i = 0; i < count; i++)
            {
                var keyPair = RSAEncryptionService.GenerateRSAKeyPair(size);
                var publicKeyString = RSAExtensions.ExportPublicKey(keyPair.Public);
                var privateKeyString = RSAExtensions.ExportPrivateKey(keyPair.Private);

                var passPhrase = GeneratePassPhrase();
                var salt = GenerateSalt();

                var rsaCreds = new RSACreds(GuidGenerator.Create())
                {
                    Identifier = GenerateIdentifier(),
                    Size = size,
                    SourceType = (int)KeySourceType.Generate,
                    PublicKey = EncryptKey(publicKeyString, passPhrase, salt),
                    PrivateKey = EncryptKey(privateKeyString, passPhrase, salt),
                    PassPhrase = passPhrase,
                    Salt = salt
                };

                rsaCredsList.Add(rsaCreds);
            }
            
            return rsaCredsList;
        }

        /// <summary>
        /// Generates a specified number of SM2 credential pairs using the specified elliptic curve.
        /// Each credential includes encrypted public and private keys with unique identifiers and secure passphrases.
        /// </summary>
        /// <param name="curve">The elliptic curve name for SM2 (e.g., sm2p256v1, wapip192v1).</param>
        /// <param name="count">The number of SM2 credential pairs to generate.</param>
        /// <returns>A list of generated SM2 credentials with encrypted keys and metadata.</returns>
        public virtual List<SM2Creds> GenerateSM2Creds(string curve, int count)
        {
            var sm2CredsList = new List<SM2Creds>();
            
            for (int i = 0; i < count; i++)
            {
                var keyPair = Sm2EncryptionService.GenerateSm2KeyPair(curve);
                var publicKeyString = Sm2Extensions.ExportPublicKey(keyPair.Public);
                var privateKeyString = Sm2Extensions.ExportPrivateKey(keyPair.Private);

                var passPhrase = GeneratePassPhrase();
                var salt = GenerateSalt();

                var sm2Creds = new SM2Creds(GuidGenerator.Create())
                {
                    Identifier = GenerateIdentifier(),
                    SourceType = (int)KeySourceType.Generate,
                    Curve = curve,
                    PublicKey = EncryptKey(publicKeyString, passPhrase, salt),
                    PrivateKey = EncryptKey(privateKeyString, passPhrase, salt),
                    PassPhrase = passPhrase,
                    Salt = salt
                };

                sm2CredsList.Add(sm2Creds);
            }
            
            return sm2CredsList;
        }

        /// <summary>
        /// Encrypts a plain text key using the specified passphrase and salt for secure storage.
        /// </summary>
        /// <param name="plainText">The plain text key to encrypt.</param>
        /// <param name="passPhrase">The passphrase used for encryption.</param>
        /// <param name="salt">The salt value used to strengthen encryption.</param>
        /// <returns>The encrypted key as a string.</returns>
        public virtual string EncryptKey(string plainText, string passPhrase, string salt)
        {
            return StringEncryptionService.Encrypt(plainText, passPhrase, Encoding.UTF8.GetBytes(salt));
        }

        /// <summary>
        /// Decrypts an encrypted key using the specified passphrase and salt to retrieve the original key.
        /// </summary>
        /// <param name="cipherText">The encrypted key to decrypt.</param>
        /// <param name="passPhrase">The passphrase used for decryption.</param>
        /// <param name="salt">The salt value used during encryption.</param>
        /// <returns>The decrypted plain text key.</returns>
        public virtual string DecryptKey(string cipherText, string passPhrase, string salt)
        {
            return StringEncryptionService.Decrypt(cipherText, passPhrase, Encoding.UTF8.GetBytes(salt));
        }

        /// <summary>
        /// Generates a unique identifier for cryptographic credentials using GUID without hyphens.
        /// </summary>
        /// <returns>A unique identifier string in format without hyphens.</returns>
        public virtual string GenerateIdentifier()
        {
            return GuidGenerator.Create().ToString("N");
        }

        /// <summary>
        /// Generates a secure passphrase for key encryption with sufficient complexity.
        /// </summary>
        /// <returns>A randomly generated passphrase string of 12 characters.</returns>
        public virtual string GeneratePassPhrase()
        {
            return GenerateSecureRandomString(12);
        }

        /// <summary>
        /// Generates a salt value for key encryption strengthening to prevent rainbow table attacks.
        /// </summary>
        /// <returns>A randomly generated salt string of 8 characters.</returns>
        public virtual string GenerateSalt()
        {
            return GenerateSecureRandomString(8);
        }

        /// <summary>
        /// Generates a cryptographically secure random string of the specified length.
        /// Uses a combination of uppercase, lowercase letters and digits for maximum entropy.
        /// </summary>
        /// <param name="length">The length of the random string to generate.</param>
        /// <returns>A cryptographically secure random string.</returns>
        protected virtual string GenerateSecureRandomString(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            
            using var rng = RandomNumberGenerator.Create();
            var result = new char[length];
            var randomBytes = new byte[length * 4]; // Extra bytes for better randomness
            
            rng.GetBytes(randomBytes);
            
            for (int i = 0; i < length; i++)
            {
                var randomIndex = BitConverter.ToUInt32(randomBytes, i * 4) % validChars.Length;
                result[i] = validChars[(int)randomIndex];
            }
            
            return new string(result);
        }
    }
}
