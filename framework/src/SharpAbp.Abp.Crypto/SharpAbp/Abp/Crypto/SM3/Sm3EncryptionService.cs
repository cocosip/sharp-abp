using Org.BouncyCastle.Crypto.Digests;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Crypto.SM3
{
    public class Sm3EncryptionService : ISm3EncryptionService, ITransientDependency
    {
        /// <summary>
        /// 使用SM3获取Hash
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <returns></returns>
        public virtual byte[] GetHash(byte[] plainText)
        {
            var sm3 = new SM3Digest();
            var v = new byte[sm3.GetDigestSize()];
            sm3.BlockUpdate(plainText, 0, plainText.Length);
            sm3.DoFinal(v, 0);
            return v;
        }

    }
}
