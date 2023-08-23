using Org.BouncyCastle.Utilities.Encoders;
using System.Text;

namespace SharpAbp.Abp.SM
{
    public static class Sm4EncryptionServiceExtensions
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="sm4EncryptionService"></param>
        /// <param name="plainText">明文</param>
        /// <param name="key">密钥(十六进制)</param>
        /// <param name="iv">IV向量(十六进制)</param>
        /// <param name="mode">加密模式,ECB或CBC</param>
        /// <param name="padding">填充模式,NoPadding,PKCS5Padding,PKCS7Padding</param>
        /// <param name="codeName">编码方式,默认使用utf-8</param>
        /// <returns></returns>
        public static string Encrypt(
            this ISm4EncryptionService sm4EncryptionService,
            string plainText,
            string key,
            string iv,
            string mode = "",
            string padding = "",
            string codeName = "utf-8")
        {
            var cipherText = sm4EncryptionService.Encrypt(
                Encoding.GetEncoding(codeName).GetBytes(plainText),
                Encoding.GetEncoding(codeName).GetBytes(key),
                Encoding.GetEncoding(codeName).GetBytes(iv),
                mode,
                padding);

            return Hex.ToHexString(cipherText);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="sm4EncryptionService"></param>
        /// <param name="cipherText">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV向量</param>
        /// <param name="mode">加密模式,ECB或CBC</param>
        /// <param name="padding">填充模式,NoPadding,PKCS5Padding,PKCS7Padding</param>
        /// <param name="codeName">编码方式,默认使用utf-8</param>
        /// <returns></returns>
        public static string Decrypt(
            this ISm4EncryptionService sm4EncryptionService,
            string cipherText,
            string key,
            string iv,
            string mode = "",
            string padding = "",
            string codeName = "utf-8")
        {
            var buffer = sm4EncryptionService.Decrypt(
                Hex.DecodeStrict(cipherText),
                Encoding.GetEncoding(codeName).GetBytes(key),
                Encoding.GetEncoding(codeName).GetBytes(iv),
                mode,
                padding);

            return Encoding.GetEncoding(codeName).GetString(buffer);
        }
    }
}
