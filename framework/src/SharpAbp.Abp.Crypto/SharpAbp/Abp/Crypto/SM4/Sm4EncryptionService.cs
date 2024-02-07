using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Crypto.SM4
{
    public class Sm4EncryptionService : ISm4EncryptionService, ITransientDependency
    {
        protected AbpSm4EncryptionOptions Options { get; }
        public Sm4EncryptionService(IOptions<AbpSm4EncryptionOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV向量</param>
        /// <param name="mode">加密模式,ECB或CBC</param>
        /// <param name="padding">填充模式,NoPadding,PKCS5Padding,PKCS7Padding</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual byte[] Encrypt(
            byte[] plainText,
            byte[] key,
            byte[] iv,
            string mode = "",
            string padding = "")
        {

            if (key.Length != 16)
            {
                throw new ArgumentException("invalid sm4 key length");
            }

            //CBC模式下,iv不为空
            if (mode.Equals(Sm4EncryptionNames.ModeCBC, StringComparison.OrdinalIgnoreCase))
            {
                iv ??= Options.DefaultIv;

                if (iv.Length != 16)
                {
                    throw new ArgumentException("invalid sm4 cbc iv length");
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
        /// 解密
        /// </summary>
        /// <param name="cipherText">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV向量</param>
        /// <param name="mode">加密模式,ECB或CBC</param>
        /// <param name="padding">填充模式,NoPadding,PKCS5Padding,PKCS7Padding</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual byte[] Decrypt(
            byte[] cipherText,
            byte[] key,
            byte[] iv,
            string mode = "",
            string padding = "")
        {
            if (key.Length != 16)
            {
                throw new ArgumentException("invalid sm4 key length");
            }

            //CBC模式下,iv不为空
            if (mode.Equals(Sm4EncryptionNames.ModeCBC, StringComparison.OrdinalIgnoreCase))
            {
                iv ??= Options.DefaultIv;

                if (iv.Length != 16)
                {
                    throw new ArgumentException("invalid sm4 cbc iv length");
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
