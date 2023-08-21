using Org.BouncyCastle.Utilities.Encoders;
using System.Text;

namespace SharpAbp.Abp.SM
{
    public static class Sm3EncryptionServiceExtensions
    {
        /// <summary>
        /// 使用SM3获取Hash
        /// </summary>
        /// <param name="sm3EncryptionService"></param>
        /// <param name="plainText">明文</param>
        /// <param name="codeName">编码,默认utf-8</param>
        /// <returns></returns>
        public static string GetHash(
            this ISm3EncryptionService sm3EncryptionService,
            string plainText, 
            string codeName = "utf-8")
        {
            var buffer = Encoding.GetEncoding(codeName).GetBytes(plainText);
            var v = sm3EncryptionService.GetHash(buffer);
            return Hex.ToHexString(v);
        }

    }
}
