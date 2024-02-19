using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using System;

namespace SharpAbp.Abp.Crypto.SM2
{
    public static class Sm2Extensions
    {
        /// <summary>
        /// 将SM2公钥参数导出成二进制字符串
        /// </summary>
        /// <param name="publicKeyParam"></param>
        /// <returns></returns>
        public static string ExportPublicKey(this AsymmetricKeyParameter publicKeyParam)
        {
            if (publicKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not public key");
            }

            var pub = ((ECPublicKeyParameters)publicKeyParam).Q.GetEncoded();
            return Hex.ToHexString(pub);
        }

        /// <summary>
        /// 将SM2私钥参数导出成二进制字符串
        /// </summary>
        /// <param name="privateKeyParam"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ExportPrivateKey(this AsymmetricKeyParameter privateKeyParam)
        {
            if (!privateKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not private key");
            }

            var priv = ((ECPrivateKeyParameters)privateKeyParam).D.ToByteArray();
            return Hex.ToHexString(priv);
        }


        /// <summary>
        /// 导出SM2密钥对的公钥导出成二进制字符串
        /// </summary>
        /// <param name="keyPair"></param>
        /// <returns></returns>
        public static string ExportPublicKey(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Public.ExportPublicKey();
        }

        /// <summary>
        /// 导出SM2密钥对的私钥导出成二进制字符串
        /// </summary>
        /// <param name="keyPair"></param>
        /// <returns></returns>
        public static string ExportPrivateKey(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Private.ExportPrivateKey();
        }

    }
}
