namespace SharpAbp.Abp.Crypto.SM4
{
    /// <summary>
    /// Defines the interface for SM4 encryption and decryption services.
    /// </summary>
    public interface ISm4EncryptionService
    {
        /// <summary>
        /// Encrypts data using the SM4 algorithm.
        /// </summary>
        /// <param name="plainText">The plain text data to encrypt.</param>
        /// <param name="key">The encryption key (16 bytes).</param>
        /// <param name="iv">The Initialization Vector (IV) (16 bytes for CBC mode).</param>
        /// <param name="mode">The encryption mode (e.g., ECB, CBC). Defaults to the configured default mode.</param>
        /// <param name="padding">The padding scheme (e.g., PKCS7Padding, NoPadding). Defaults to the configured default padding.</param>
        /// <returns>The encrypted data as a byte array.</returns>
        byte[] Encrypt(byte[] plainText, byte[] key, byte[] iv, string mode = "", string padding = "");

        /// <summary>
        /// Decrypts data using the SM4 algorithm.
        /// </summary>
        /// <param name="cipherText">The encrypted data to decrypt.</param>
        /// <param name="key">The decryption key (16 bytes).</param>
        /// <param name="iv">The Initialization Vector (IV) (16 bytes for CBC mode).</param>
        /// <param name="mode">The decryption mode (e.g., ECB, CBC). Defaults to the configured default mode.</param>
        /// <param name="padding">The padding scheme (e.g., PKCS7Padding, NoPadding). Defaults to the configured default padding.</param>
        /// <returns>The decrypted data as a byte array.</returns>
        byte[] Decrypt(byte[] cipherText, byte[] key, byte[] iv, string mode = "", string padding = "");
    }
}
