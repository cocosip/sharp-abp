namespace SharpAbp.Abp.Crypto.SM3
{
    public interface ISm3EncryptionService
    {
        /// <summary>
        /// Computes the SM3 hash of the given plain text.
        /// </summary>
        /// <param name="plainText">The plain text data to hash.</param>
        /// <returns>The SM3 hash as a byte array.</returns>
        byte[] GetHash(byte[] plainText);
    }
}
