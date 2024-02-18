using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.IO;

namespace SharpAbp.Abp.Crypto.RSA
{
    public interface IRSAEncryptionService
    {
        /// <summary>
        /// 生成RSA密钥对
        /// </summary>
        /// <param name="keySize"></param>
        /// <param name="rd"></param>
        /// <returns></returns>
        AsymmetricCipherKeyPair GenerateRSAKeyPair(int keySize = 2048, SecureRandom rd = null);

        /// <summary>
        /// 导入RSA公钥 (原始RSA公钥为Asn1,DER格式)
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        AsymmetricKeyParameter ImportPublicKey(Stream stream);

        /// <summary>
        /// 从Pem中导入RSA公钥
        /// </summary>
        /// <param name="publicKeyPem"></param>
        /// <returns></returns>
        AsymmetricKeyParameter ImportPublicKeyPem(string publicKeyPem);

        /// <summary>
        /// 从Stream中导入RSA私钥 (原始RSA公钥为Asn1,DER格式)
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        AsymmetricKeyParameter ImportPrivateKey(Stream stream);

        /// <summary>
        /// 从Stream中导入PKCS8格式RSA私钥
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        AsymmetricKeyParameter ImportPrivateKeyPkcs8(Stream stream);

        /// <summary>
        /// 从Pem中导入RSA私钥
        /// </summary>
        /// <param name="privateKeyPem"></param>
        /// <returns></returns>
        AsymmetricKeyParameter ImportPrivateKeyPem(string privateKeyPem);

        /// <summary>
        /// 从Pem中导入PKCS8格式RSA私钥
        /// </summary>
        /// <param name="privateKeyPem"></param>
        /// <returns></returns>
        AsymmetricKeyParameter ImportPrivateKeyPkcs8Pem(string privateKeyPem);

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="publicKeyParam"></param>
        /// <param name="plainText"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        byte[] Encrypt(AsymmetricKeyParameter publicKeyParam, byte[] plainText);

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="privateKeyParam"></param>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        byte[] Decrypt(AsymmetricKeyParameter privateKeyParam, byte[] cipherText);

        /// <summary>
        /// RSA加签
        /// </summary>
        /// <param name="privateKeyParam"></param>
        /// <param name="data"></param>
        /// <param name="algorithm">SHA1WITHRSA,SHA256WITHRSA,SHA384WITHRSA,SHA512WITHRSA</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        byte[] Sign(AsymmetricKeyParameter privateKeyParam, byte[] data, string algorithm = "SHA256WITHRSA");

        /// <summary>
        /// RSA验签
        /// </summary>
        /// <param name="privateKeyParam"></param>
        /// <param name="data"></param>
        /// <param name="signature"></param>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        bool VerifySign(AsymmetricKeyParameter privateKeyParam, byte[] data, byte[] signature, string algorithm = "SHA256WITHRSA");
    }
}
