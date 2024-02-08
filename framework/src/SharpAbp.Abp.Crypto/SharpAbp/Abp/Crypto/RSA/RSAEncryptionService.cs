using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using Volo.Abp.DependencyInjection;


namespace SharpAbp.Abp.Crypto.RSA
{
    public class RSAEncryptionService : IRSAEncryptionService, ITransientDependency
    {
        /// <summary>
        /// 生成RSA密钥对
        /// </summary>
        /// <param name="keySize"></param>
        /// <param name="rd"></param>
        /// <returns></returns>
        public virtual AsymmetricCipherKeyPair GenerateRSAKeyPair(int keySize = 2048, SecureRandom rd = null)
        {
            rd ??= new SecureRandom();
            var generator = new RsaKeyPairGenerator();
            generator.Init(new KeyGenerationParameters(rd, keySize));
            return generator.GenerateKeyPair();
        }

        /// <summary>
        /// 从Stream中导入RSA公钥 (原始RSA公钥为Asn1,DER格式)
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public virtual AsymmetricKeyParameter ImportPublicKey(Stream stream)
        {
            Asn1Object asn1Object = Asn1Object.FromStream(stream);
            var publicKeyInfo = SubjectPublicKeyInfo.GetInstance(asn1Object);
            return PublicKeyFactory.CreateKey(publicKeyInfo);
        }

        /// <summary>
        /// 从Pem中导入RSA公钥
        /// </summary>
        /// <param name="publicKeyPem"></param>
        /// <returns></returns>
        public virtual AsymmetricKeyParameter ImportPublicKeyPem(string publicKeyPem)
        {
            using var stringReader = new StringReader(publicKeyPem);
            using var pemReader = new PemReader(stringReader);
            return (AsymmetricKeyParameter)pemReader.ReadObject();
        }

        /// <summary>
        /// 从Stream中导入RSA私钥 (原始RSA公钥为Asn1,DER格式)
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public virtual AsymmetricKeyParameter ImportPrivateKey(Stream stream)
        {

            //Asn1Object asn1Object = Asn1Object.FromStream(stream);

            var pkcs1PrivateKeyStructure = RsaPrivateKeyStructure.GetInstance(stream);

            var privateKeyParam = new RsaPrivateCrtKeyParameters(
                pkcs1PrivateKeyStructure.Modulus,
                pkcs1PrivateKeyStructure.PublicExponent,
                pkcs1PrivateKeyStructure.PrivateExponent,
                pkcs1PrivateKeyStructure.Prime1,
                pkcs1PrivateKeyStructure.Prime2,
                pkcs1PrivateKeyStructure.Exponent1,
                pkcs1PrivateKeyStructure.Exponent2,
                pkcs1PrivateKeyStructure.Coefficient);
            return privateKeyParam;


            //var publicKeyParam = PublicKeyFactory.CreateKey(privateKeyParam);

            //var publicKeyParam = new Publick(pkcs1PrivateKeyStructure.Modulus, pkcs1PrivateKeyStructure.PublicExponent);

            //return new AsymmetricCipherKeyPair(publicKeyParam, privateKeyParam);

            //Asn1Object asn1Object = Asn1Object.FromStream(stream);
            //var privateKeyInfo = PrivateKeyInfo.GetInstance(asn1Object);
            //var privateKeyParam = PrivateKeyFactory.CreateKey(privateKeyInfo);
            //return privateKeyParam;
        }

        /// <summary>
        /// 从Stream中导入PKCS8格式RSA私钥
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public virtual AsymmetricKeyParameter ImportPrivateKeyPkcs8(Stream stream)
        {
            Asn1Object asn1Object = Asn1Object.FromStream(stream);
            var privateKeyInfo = PrivateKeyInfo.GetInstance(asn1Object);
            var privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(privateKeyInfo);
            return privateKeyParam;
        }

        /// <summary>
        /// 从Pem中导入RSA私钥
        /// </summary>
        /// <param name="privateKeyPem"></param>
        /// <returns></returns>
        public virtual AsymmetricKeyParameter ImportPrivateKeyPem(string privateKeyPem)
        {
            using var stringReader = new StringReader(privateKeyPem);
            using var pemReader = new PemReader(stringReader);
            return (AsymmetricKeyParameter)pemReader.ReadObject();
        }

        /// <summary>
        /// 从Pem中导入PKCS8格式RSA私钥
        /// </summary>
        /// <param name="privateKeyPem"></param>
        /// <returns></returns>
        public virtual AsymmetricKeyParameter ImportPrivateKeyPemPkcs8(string privateKeyPem)
        {
            using var stringReader = new StringReader(privateKeyPem);
            using var pemReader = new PemReader(stringReader);
            var privateKeyParam = (RsaPrivateCrtKeyParameters)pemReader.ReadObject();
            return privateKeyParam;
        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="publicKeyParam"></param>
        /// <param name="plainText"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual byte[] Encrypt(AsymmetricKeyParameter publicKeyParam, byte[] plainText)
        {
            if (publicKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not public key");
            }
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());
            encryptEngine.Init(true, publicKeyParam);
            return encryptEngine.ProcessBlock(plainText, 0, plainText.Length);
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="privateKeyParam"></param>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual byte[] Decrypt(AsymmetricKeyParameter privateKeyParam, byte[] cipherText)
        {
            if (!privateKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not private key");
            }
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());
            encryptEngine.Init(false, privateKeyParam);
            return encryptEngine.ProcessBlock(cipherText, 0, cipherText.Length);
        }

        /// <summary>
        /// RSA加签
        /// </summary>
        /// <param name="privateKeyParam"></param>
        /// <param name="data"></param>
        /// <param name="algorithm">SHA1WITHRSA,SHA256WITHRSA,SHA384WITHRSA,SHA512WITHRSA</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual byte[] Sign(AsymmetricKeyParameter privateKeyParam, byte[] data, string algorithm = "SHA256WITHRSA")
        {
            if (!privateKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not private key");
            }

            ISigner signer = SignerUtilities.GetSigner(algorithm);
            signer.Init(true, privateKeyParam);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.GenerateSignature();
        }

        /// <summary>
        /// RSA验签
        /// </summary>
        /// <param name="privateKeyParam"></param>
        /// <param name="data"></param>
        /// <param name="signature"></param>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual bool VerifySign(AsymmetricKeyParameter privateKeyParam, byte[] data, byte[] signature, string algorithm = "SHA256WITHRSA")
        {
            if (privateKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not public key");
            }

            ISigner signer = SignerUtilities.GetSigner(algorithm);
            signer.Init(true, privateKeyParam);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(signature);
        }


    }
}
