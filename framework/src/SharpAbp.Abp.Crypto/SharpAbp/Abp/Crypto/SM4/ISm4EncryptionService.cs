namespace SharpAbp.Abp.Crypto.SM4
{
    public interface ISm4EncryptionService
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV向量</param>
        /// <param name="mode">加密模式,ECB或CBC</param>
        /// <param name="padding">填充模式,NoPadding,PKCS5Padding,PKCS7Padding</param>
        /// <returns></returns>
        byte[] Encrypt(byte[] plainText, byte[] key, byte[] iv, string mode = "", string padding = "");

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cipherText">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV向量</param>
        /// <param name="mode">加密模式,ECB或CBC</param>
        /// <param name="padding">填充模式,NoPadding,PKCS5Padding,PKCS7Padding</param>
        /// <returns></returns>
        byte[] Decrypt(byte[] cipherText, byte[] key, byte[] iv, string mode = "", string padding = "");
    }
}
