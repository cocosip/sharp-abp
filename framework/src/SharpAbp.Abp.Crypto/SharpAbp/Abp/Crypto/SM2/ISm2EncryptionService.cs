using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SharpAbp.Abp.Crypto.SM2
{
    public interface ISm2EncryptionService
    {
        /// <summary>
        /// 生成SM2密钥对
        /// </summary>
        /// <param name="curve">曲率名称,默认使用:Sm2p256v1</param>
        /// <param name="rd">随机数</param>
        /// <returns></returns>
        AsymmetricCipherKeyPair GenerateSm2KeyPair(string curve = Sm2EncryptionNames.CurveSm2p256v1, SecureRandom? rd = null);

        /// <summary>
        /// 使用公钥进行加密
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="plainText">明文</param>
        /// <param name="curve">曲率名称</param>
        /// <param name="mode">加密模式,默认使用:C1C2C3</param>
        /// <returns></returns>
        byte[] Encrypt(byte[] publicKey, byte[] plainText, string curve = Sm2EncryptionNames.CurveSm2p256v1, Mode mode = Mode.C1C2C3);

        /// <summary>
        /// 使用私钥进行解密
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="cipherText">密文</param>
        /// <param name="curve">曲率名称</param>
        /// <param name="mode">加密模式</param>
        /// <returns></returns>
        byte[] Decrypt(byte[] privateKey, byte[] cipherText, string curve = Sm2EncryptionNames.CurveSm2p256v1, Mode mode = Mode.C1C2C3);

        /// <summary>
        /// 使用私钥进行加签
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="plainText">明文</param>
        /// <param name="curve">曲率名称</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        byte[] Sign(byte[] privateKey, byte[] plainText, string curve = Sm2EncryptionNames.CurveSm2p256v1, byte[]? id = null);

        /// <summary>
        /// 使用公钥进行验签
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="plainText">明文</param>
        /// <param name="signature">签名数据</param>
        /// <param name="curve">曲率名称</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        bool VerifySign(byte[] publicKey, byte[] plainText, byte[] signature, string curve = Sm2EncryptionNames.CurveSm2p256v1, byte[]? id = null);

        /// <summary>
        ///  将C1C22C3编码转换成C1C3C2编码
        /// </summary>
        /// <param name="c1c2c3">C1C2C3</param>
        /// <param name="curve">曲率名称</param>
        /// <returns></returns>
        byte[] C123ToC132(byte[] c1c2c3, string curve = Sm2EncryptionNames.CurveSm2p256v1);

        /// <summary>
        /// 将C1C3C2编码转换成C1C2C3
        /// </summary>
        /// <param name="c1c3c2">C1C3C2</param>
        /// <param name="curve">曲率名称</param>
        /// <returns></returns>
        byte[] C132ToC123(byte[] c1c3c2, string curve = Sm2EncryptionNames.CurveSm2p256v1);

    }
}
