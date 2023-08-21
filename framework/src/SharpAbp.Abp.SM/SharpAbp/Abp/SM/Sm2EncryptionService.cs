using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using Volo.Abp.DependencyInjection;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SharpAbp.Abp.SM
{
    public class Sm2EncryptionService : ISm2EncryptionService, ITransientDependency
    {
        protected AbpSm2EncryptionOptions Options { get; }

        public Sm2EncryptionService(IOptions<AbpSm2EncryptionOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// 生成SM2密钥对
        /// </summary>
        /// <param name="curve">曲率名称,默认使用:Sm2p256v1</param>
        /// <param name="rd">随机数</param>
        /// <returns></returns>
        public virtual AsymmetricCipherKeyPair GenerateKeyPair(string curve = Sm2EncryptionNames.CurveSm2p256v1, SecureRandom rd = null)
        {
            rd ??= new SecureRandom();
            if (curve.IsNullOrWhiteSpace())
            {
                curve = Options.DefaultCurve;
            }

            var generator = new ECKeyPairGenerator();
            generator.Init(new ECKeyGenerationParameters(new ECDomainParameters(GMNamedCurves.GetByName(curve)), rd));
            return generator.GenerateKeyPair();
        }

        /// <summary>
        /// 使用公钥进行加密
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="plainText">明文</param>
        /// <param name="curve">曲率名称</param>
        /// <param name="mode">加密模式,默认使用:C1C2C3</param>
        /// <returns></returns>
        public virtual byte[] Encrypt(byte[] publicKey, byte[] plainText, string curve = Sm2EncryptionNames.CurveSm2p256v1, Mode mode = Mode.C1C2C3)
        {
            if (curve.IsNullOrWhiteSpace())
            {
                curve = Options.DefaultCurve;
            }

            var engine = new SM2Engine(mode);
            var x9ec = GMNamedCurves.GetByName(curve);
            var p = new ECPublicKeyParameters(x9ec.Curve.DecodePoint(publicKey), new ECDomainParameters(x9ec));
            engine.Init(true, new ParametersWithRandom(p));
            var v = engine.ProcessBlock(plainText, 0, plainText.Length);
            return v;
        }

        /// <summary>
        /// 使用私钥进行解密
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="cipherText">密文</param>
        /// <param name="curve">曲率名称</param>
        /// <param name="mode">加密模式</param>
        /// <returns></returns>
        public virtual byte[] Decrypt(byte[] privateKey, byte[] cipherText, string curve = Sm2EncryptionNames.CurveSm2p256v1, Mode mode = Mode.C1C2C3)
        {
            if (curve.IsNullOrWhiteSpace())
            {
                curve = Options.DefaultCurve;
            }

            var engine = new SM2Engine(mode);
            var x9ec = GMNamedCurves.GetByName(curve);
            var p = new ECPrivateKeyParameters(new BigInteger(privateKey), new ECDomainParameters(x9ec));
            engine.Init(false, p);

            var v = engine.ProcessBlock(cipherText, 0, cipherText.Length);
            return v;
        }

        /// <summary>
        /// 使用私钥进行加签
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="plainText">明文</param>
        /// <param name="curve">曲率名称</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        public virtual byte[] Sign(byte[] privateKey, byte[] plainText, string curve = Sm2EncryptionNames.CurveSm2p256v1, byte[] id = null)
        {
            if (curve.IsNullOrWhiteSpace())
            {
                curve = Options.DefaultCurve;
            }

            var x9ec = GMNamedCurves.GetByName(curve);
            var p = new ECPrivateKeyParameters(new BigInteger(privateKey), new ECDomainParameters(x9ec));

            var signer = new SM2Signer();
            ICipherParameters cp = id != null ? new ParametersWithID(new ParametersWithRandom(p), id) : new ParametersWithRandom(p);

            signer.Init(true, cp);
            signer.BlockUpdate(plainText, 0, plainText.Length);
            return signer.GenerateSignature();
        }

        /// <summary>
        /// 使用公钥进行验签
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="plainText">明文</param>
        /// <param name="signature">签名数据</param>
        /// <param name="curve">曲率名称</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        public virtual bool VerifySign(byte[] publicKey, byte[] plainText, byte[] signature, string curve = Sm2EncryptionNames.CurveSm2p256v1, byte[] id = null)
        {
            if (curve.IsNullOrWhiteSpace())
            {
                curve = Options.DefaultCurve;
            }
            var x9ec = GMNamedCurves.GetByName(curve);
            var p = new ECPublicKeyParameters(x9ec.Curve.DecodePoint(publicKey), new ECDomainParameters(x9ec));
            var signer = new SM2Signer();

            ICipherParameters cp = id != null ? new ParametersWithID(new ParametersWithRandom(p), id) : new ParametersWithRandom(p);
            signer.Init(false, cp);
            signer.BlockUpdate(plainText, 0, plainText.Length);
            return signer.VerifySignature(signature);
        }

        /// <summary>
        ///  将C1C22C3编码转换成C1C3C2编码
        /// </summary>
        /// <param name="c1c2c3">C1C2C3</param>
        /// <param name="curve">曲率名称</param>
        /// <returns></returns>
        public virtual byte[] C123ToC132(byte[] c1c2c3, string curve = Sm2EncryptionNames.CurveSm2p256v1)
        {
            if (curve.IsNullOrWhiteSpace())
            {
                curve = Options.DefaultCurve;
            }
            var x9ec = GMNamedCurves.GetByName(curve);
            var c1Len = (x9ec.Curve.FieldSize + 7) / 8 * 2 + 1;
            var c3Len = 32;
            var result = new byte[c1c2c3.Length];
            Array.Copy(c1c2c3, 0, result, 0, c1Len);
            Array.Copy(c1c2c3, c1c2c3.Length - c3Len, result, c1Len, c3Len);
            Array.Copy(c1c2c3, c1Len, result, c1Len + c3Len, c1c2c3.Length - c1Len - c3Len);
            return result;
        }

        /// <summary>
        /// 将C1C3C2编码转换成C1C2C3
        /// </summary>
        /// <param name="c1c3c2">C1C3C2</param>
        /// <param name="curve">曲率名称</param>
        /// <returns></returns>
        public virtual byte[] C132ToC123(byte[] c1c3c2, string curve = Sm2EncryptionNames.CurveSm2p256v1)
        {
            if (curve.IsNullOrWhiteSpace())
            {
                curve = Options.DefaultCurve;
            }
            var x9ec = GMNamedCurves.GetByName(curve);
            var c1Len = (x9ec.Curve.FieldSize + 7) / 8 * 2 + 1;
            var c3Len = 32;
            var result = new byte[c1c3c2.Length];
            Array.Copy(c1c3c2, 0, result, 0, c1Len);
            Array.Copy(c1c3c2, c1Len + c3Len, result, c1Len, c1c3c2.Length - c1Len - c3Len);
            Array.Copy(c1c3c2, c1Len, result, c1c3c2.Length - c3Len, c3Len);
            return result;
        }
    }
}
