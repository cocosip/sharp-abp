using Org.BouncyCastle.Crypto.Digests;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Crypto.SM3
{
    /// <summary>
    /// Provides SM3 hashing services.
    /// </summary>
    public class Sm3EncryptionService : ISm3EncryptionService, ITransientDependency
    {
        /// <summary>
        /// Computes the SM3 hash of the given plain text.
        /// </summary>
        /// <param name="plainText">The plain text data to hash.</param>
        /// <returns>The SM3 hash as a byte array.</returns>
        public virtual byte[] GetHash(byte[] plainText)
        {
            var sm3 = new SM3Digest();
            var result = new byte[sm3.GetDigestSize()];
            sm3.BlockUpdate(plainText, 0, plainText.Length);
            sm3.DoFinal(result, 0);
            return result;
        }
    }
}
