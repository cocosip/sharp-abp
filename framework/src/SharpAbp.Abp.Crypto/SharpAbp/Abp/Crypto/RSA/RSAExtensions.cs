using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;
using System;
using System.IO;

namespace SharpAbp.Abp.Crypto.RSA
{
    public static class RSAExtensions
    {

        /// <summary>
        /// 导出公钥
        /// </summary>
        /// <param name="publicKeyParam"></param>
        /// <returns></returns>
        public static string ExportPublicKey(this AsymmetricKeyParameter publicKeyParam)
        {
            if (publicKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not public key");
            }
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKeyParam);
            return Base64.ToBase64String(publicKeyInfo.GetEncoded());
        }

        /// <summary>
        ///  导出公钥的Pem格式 (PKCS1)
        /// </summary>
        /// <param name="publicKeyParam"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ExportPublicKeyToPem(this AsymmetricKeyParameter publicKeyParam)
        {
            if (publicKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not public key");
            }
            using var stringWriter = new StringWriter();
            using var pemWriter = new PemWriter(stringWriter);
            pemWriter.WriteObject(publicKeyParam);
            pemWriter.Writer.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// 导出私钥的Base64格式 (PKCS1原始)
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

            //var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParam);
            //return Base64.ToBase64String(privateKeyInfo.GetEncoded());

            // 创建PrivateKeyInfo对象
            var rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)privateKeyParam;
            var rsaPrivateKeyStructure = new RsaPrivateKeyStructure(
                rsaPrivateCrtKeyParameters.Modulus,
                rsaPrivateCrtKeyParameters.PublicExponent,
                rsaPrivateCrtKeyParameters.Exponent,
                rsaPrivateCrtKeyParameters.P,
                rsaPrivateCrtKeyParameters.Q,
                rsaPrivateCrtKeyParameters.DP,
                rsaPrivateCrtKeyParameters.DQ,
                rsaPrivateCrtKeyParameters.QInv);

            return Convert.ToBase64String(rsaPrivateKeyStructure.GetDerEncoded());
        }


        /// <summary>
        /// 导出私钥的PKCS8的Base64格式 (PKCS1)
        /// </summary>
        /// <param name="privateKeyParam"></param>
        /// <returns></returns>
        public static string ExportPrivateKeyToPem(this AsymmetricKeyParameter privateKeyParam)
        {
            using var stringWriter = new StringWriter();
            using var pemWriter = new PemWriter(stringWriter);
            pemWriter.WriteObject(privateKeyParam);
            pemWriter.Writer.Close();
            return stringWriter.ToString();
        }


        /// <summary>
        /// 导出私钥的PKCS8的Base64格式 (PKCS8)
        /// </summary>
        /// <param name="privateKeyParam"></param>
        /// <returns></returns>
        public static string ExportPrivateKeyPkcs8(this AsymmetricKeyParameter privateKeyParam)
        {
            if (!privateKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not private key");
            }
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParam);
            return Base64.ToBase64String(privateKeyInfo.GetDerEncoded());
        }

        /// <summary>
        /// 导出私钥的Pem格式 (PKCS8)
        /// </summary>
        /// <param name="privateKeyParam"></param>
        /// <returns></returns>
        public static string ExportPrivateKeyPkcs8ToPem(this AsymmetricKeyParameter privateKeyParam)
        {
            using var stringWriter = new StringWriter();
            using var pemWriter = new PemWriter(stringWriter);
            var gen = new Pkcs8Generator(privateKeyParam);
            pemWriter.WriteObject(gen);
            pemWriter.Writer.Close();
            return stringWriter.ToString();
        }


        /// <summary>
        /// 将RSA密钥对,导出公钥的Base64格式 (PKCS1原始)
        /// </summary>
        /// <param name="keyPair"></param>
        /// <returns></returns>
        public static string ExportPublicKey(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Public.ExportPublicKey();
        }

        /// <summary>
        /// 将RSA密钥对,导出公钥的Pem格式 (PKCS1)
        /// </summary>
        /// <param name="keyPair"></param>
        /// <returns></returns>
        public static string ExportPublicKeyToPem(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Public.ExportPublicKeyToPem();
        }

        /// <summary>
        /// 将RSA密钥对,导出私钥的Base64格式 (PKCS1原始)
        /// </summary>
        /// <param name="keyPair"></param>
        /// <returns></returns>
        public static string ExportPrivateKey(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Private.ExportPrivateKey();
        }

        /// <summary>
        /// 将RSA密钥对，导出私钥的Pem格式 (PKCS1原始)
        /// </summary>
        /// <param name="privateKeyParam"></param>
        /// <returns></returns>
        public static string ExportPrivateKeyToPem(this AsymmetricCipherKeyPair privateKeyParam)
        {
            return privateKeyParam.Private.ExportPrivateKeyToPem();
        }

        /// <summary>
        /// 将RSA密钥对,导出私钥的PKCS8的Base64格式 (PKCS8)
        /// </summary>
        /// <param name="keyPair"></param>
        /// <returns></returns>
        public static string ExportPrivateKeyPkcs8(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Private.ExportPrivateKeyPkcs8();
        }

        /// <summary>
        /// 将RSA密钥对,导出私钥的Pem格式 (PKCS8)
        /// </summary>
        /// <param name="keyPair"></param>
        /// <returns></returns>
        public static string ExportPrivateKeyPkcs8ToPem(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Private.ExportPrivateKeyPkcs8ToPem();
        }

        /// <summary>
        /// 从私钥参数中获取公钥参数
        /// </summary>
        /// <param name="privateKeyParam"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static AsymmetricKeyParameter GetPublic(this AsymmetricKeyParameter privateKeyParam)
        {
            if (!privateKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not private");
            }

            var p = (RsaPrivateCrtKeyParameters)privateKeyParam;
            var publicKeyParam = new RsaKeyParameters(false, p.Modulus, p.PublicExponent);
            return publicKeyParam;
        }

    }
}
