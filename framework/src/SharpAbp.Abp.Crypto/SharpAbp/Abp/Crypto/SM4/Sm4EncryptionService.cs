using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Crypto.SM4
{
    /// <summary>
    /// Provides SM4 encryption and decryption services.
    /// </summary>
    public class Sm4EncryptionService : ISm4EncryptionService, ITransientDependency
    {
        protected AbpSm4EncryptionOptions Options { get; }

        public Sm4EncryptionService(IOptions<AbpSm4EncryptionOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Encrypts data using the SM4 algorithm.
        /// </summary>
        /// <param name="plainText">The plain text data to encrypt.</param>
        /// <param name="key">The encryption key (16 bytes).</param>
        /// <param name="iv">The Initialization Vector (IV) (16 bytes for CBC mode).</param>
        /// <param name="mode">The encryption mode (e.g., ECB, CBC). Defaults to the configured default mode.</param>
        /// <param name="padding">The padding scheme (e.g., PKCS7Padding, NoPadding). Defaults to the configured default padding.</param>
        /// <returns>The encrypted data as a byte array.</returns>
        /// <exception cref="ArgumentException">Thrown when key or IV length is invalid.</exception>
        public virtual byte[] Encrypt(
            byte[] plainText,
            byte[] key,
            byte[] iv,
            string mode = "",
            string padding = "")
        {
            if (plainText == null)
            {
                throw new ArgumentNullException(nameof(plainText));
            }
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (key.Length != 16)
            {
                throw new ArgumentException("Invalid SM4 key length. Key must be 16 bytes.", nameof(key));
            }

            mode = string.IsNullOrWhiteSpace(mode) ? Options.DefaultMode! : mode;
            padding = string.IsNullOrWhiteSpace(padding) ? Options.DefaultPadding! : padding;

            // For CBC mode, IV is required and must be 16 bytes
            if (mode.Equals(Sm4EncryptionNames.ModeCBC, StringComparison.OrdinalIgnoreCase))
            {
                iv ??= Options.DefaultIv!;
                if (iv.Length != 16)
                {
                    throw new ArgumentException("Invalid SM4 CBC IV length. IV must be 16 bytes for CBC mode.", nameof(iv));
                }
            }

            var keyParam = ParameterUtilities.CreateKeyParameter("SM4", key);
            var algo = $"SM4/{mode}/{padding}";
            var cipher = CipherUtilities.GetCipher(algo);

            if (mode.Equals(Sm4EncryptionNames.ModeCBC, StringComparison.OrdinalIgnoreCase))
            {
                var ivParam = new ParametersWithIV(keyParam, iv);
                cipher.Init(true, ivParam);
            }
            else
            {
                cipher.Init(true, keyParam);
            }

            var buffer = cipher.DoFinal(plainText);
            return buffer;
        }

        /// <summary>
        /// Decrypts data using the SM4 algorithm.
        /// </summary>
        /// <param name="cipherText">The encrypted data to decrypt.</param>
        /// <param name="key">The decryption key (16 bytes).</param>
        /// <param name="iv">The Initialization Vector (IV) (16 bytes for CBC mode).</param>
        /// <param name="mode">The decryption mode (e.g., ECB, CBC). Defaults to the configured default mode.</param>
        /// <param name="padding">The padding scheme (e.g., PKCS7Padding, NoPadding). Defaults to the configured default padding.</param>
        /// <returns>The decrypted data as a byte array.</returns>
        /// <exception cref="ArgumentException">Thrown when key or IV length is invalid.</exception>
        public virtual byte[] Decrypt(
            byte[] cipherText,
            byte[] key,
            byte[] iv,
            string mode = "",
            string padding = "")
        {
            if (cipherText == null)
            {
                throw new ArgumentNullException(nameof(cipherText));
            }
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (key.Length != 16)
            {
                throw new ArgumentException("Invalid SM4 key length. Key must be 16 bytes.", nameof(key));
            }

            mode = string.IsNullOrWhiteSpace(mode) ? Options.DefaultMode! : mode;
            padding = string.IsNullOrWhiteSpace(padding) ? Options.DefaultPadding! : padding;

            // For CBC mode, IV is required and must be 16 bytes
            if (mode.Equals(Sm4EncryptionNames.ModeCBC, StringComparison.OrdinalIgnoreCase))
            {
                if (iv == null)
                {
                    iv = Options.DefaultIv!;
                }
                if (iv.Length != 16)
                {
                    throw new ArgumentException("Invalid SM4 CBC IV length. IV must be 16 bytes for CBC mode.", nameof(iv));
                }
            }

            var keyParam = ParameterUtilities.CreateKeyParameter("SM4", key);
            var algo = $"SM4/{mode}/{padding}";
            var cipher = CipherUtilities.GetCipher(algo);

            if (mode.Equals(Sm4EncryptionNames.ModeCBC, StringComparison.OrdinalIgnoreCase))
            {
                var ivParam = new ParametersWithIV(keyParam, iv);
                cipher.Init(false, ivParam);
            }
            else
            {
                cipher.Init(false, keyParam);
            }

            var buffer = cipher.DoFinal(cipherText);
            return buffer;
        }
    }
}
