using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities.Encoders;
using System.IO;
using System.Text;

namespace SharpAbp.Abp.Crypto.RSA
{
    public static class RSAEncryptionServiceExtensions
    {
        /// <summary>
        /// 从公钥二进制中导入RSA公钥 (原始RSA公钥为Asn1,DER格式)
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="publicKeyBytes"></param>
        /// <returns></returns>
        public static AsymmetricKeyParameter ImportPublicKey(this IRSAEncryptionService rsaEncryptionService, byte[] publicKeyBytes)
        {
            return rsaEncryptionService.ImportPublicKey(new MemoryStream(publicKeyBytes));
        }

        /// <summary>
        /// 从公钥字符串中导入RSA公钥 (原始RSA公钥为Asn1,DER格式)
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static AsymmetricKeyParameter ImportPublicKey(this IRSAEncryptionService rsaEncryptionService, string publicKey)
        {
            return rsaEncryptionService.ImportPublicKey(Base64.Decode(publicKey));
        }

        /// <summary>
        /// 从私钥二进制中导入RSA私钥 (原始RSA私钥为Asn1,DER格式)
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="privateKeyBytes"></param>
        /// <returns></returns>
        public static AsymmetricKeyParameter ImportPrivateKey(this IRSAEncryptionService rsaEncryptionService, byte[] privateKeyBytes)
        {
            return rsaEncryptionService.ImportPrivateKey(new MemoryStream(privateKeyBytes));
        }

        /// <summary>
        /// 从私钥字符串中导入RSA私钥 (原始RSA私钥为Asn1,DER格式)
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static AsymmetricKeyParameter ImportPrivateKey(this IRSAEncryptionService rsaEncryptionService, string privateKey)
        {
            return rsaEncryptionService.ImportPrivateKey(Base64.Decode(privateKey));
        }


        /// <summary>
        /// 从私钥二进制中导入PKCS8格式RSA私钥
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="privateKeyBytes"></param>
        /// <returns></returns>
        public static AsymmetricKeyParameter ImportPrivateKeyPkcs8(this IRSAEncryptionService rsaEncryptionService, byte[] privateKeyBytes)
        {
            return rsaEncryptionService.ImportPrivateKeyPkcs8(new MemoryStream(privateKeyBytes));
        }

        /// <summary>
        /// 从私钥字符串中导入PKCS8格式RSA私钥
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static AsymmetricKeyParameter ImportPrivateKeyPkcs8(this IRSAEncryptionService rsaEncryptionService, string privateKey)
        {
            return rsaEncryptionService.ImportPrivateKeyPkcs8(Base64.Decode(privateKey));
        }

        /// <summary>
        /// RSA加密为Base64字符串
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="publicKeyParam"></param>
        /// <param name="plainText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Encrypt(this IRSAEncryptionService rsaEncryptionService, AsymmetricKeyParameter publicKeyParam, string plainText, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var cipherText = rsaEncryptionService.Encrypt(publicKeyParam, encoding.GetBytes(plainText));
            return Base64.ToBase64String(cipherText);
        }

        /// <summary>
        /// RSA加密为Base64字符串
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="publicKey"></param>
        /// <param name="plainText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Encrypt(this IRSAEncryptionService rsaEncryptionService, string publicKey, string plainText, Encoding encoding = null)
        {
            var publicKeyParam = rsaEncryptionService.ImportPublicKey(publicKey);
            return rsaEncryptionService.Encrypt(publicKeyParam, plainText, encoding);
        }

        /// <summary>
        /// Pem格式RSA公钥进行加密为Base64字符串
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="publicKeyPem"></param>
        /// <param name="plainText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string EncryptFromPem(this IRSAEncryptionService rsaEncryptionService, string publicKeyPem, string plainText, Encoding encoding = null)
        {
            var publicKeyParam = rsaEncryptionService.ImportPublicKeyPem(publicKeyPem);
            return rsaEncryptionService.Encrypt(publicKeyParam, plainText, encoding);
        }


        /// <summary>
        /// RSA从Base64加密字符串中解密
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="privateKeyParam"></param>
        /// <param name="cipherText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Decrypt(this IRSAEncryptionService rsaEncryptionService, AsymmetricKeyParameter privateKeyParam, string cipherText, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var cipherBytes = Base64.Decode(cipherText);
            var plainText = rsaEncryptionService.Decrypt(privateKeyParam, cipherBytes);
            return encoding.GetString(plainText);
        }

        /// <summary>
        /// RSA从Base64加密字符串中解密
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="privateKey"></param>
        /// <param name="cipherText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Decrypt(this IRSAEncryptionService rsaEncryptionService, string privateKey, string cipherText, Encoding encoding = null)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKey(privateKey);
            return rsaEncryptionService.Decrypt(privateKeyParam, cipherText, encoding);
        }

        /// <summary>
        /// RSA使用PKCS8私钥从Base64加密字符串中解密
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="privateKey"></param>
        /// <param name="cipherText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string DecryptFromPkcs8(this IRSAEncryptionService rsaEncryptionService, string privateKey, string cipherText, Encoding encoding = null)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKeyPkcs8(privateKey);
            return rsaEncryptionService.Decrypt(privateKeyParam, cipherText, encoding);
        }

        /// <summary>
        /// RSA使用Pem私钥从Base64加密字符串中解密
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="privateKeyPem"></param>
        /// <param name="cipherText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string DecryptFromPem(this IRSAEncryptionService rsaEncryptionService, string privateKeyPem, string cipherText, Encoding encoding = null)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKeyPem(privateKeyPem);
            return rsaEncryptionService.Decrypt(privateKeyParam, cipherText, encoding);
        }

        /// <summary>
        /// RSA使用PKCS8格式的Pem私钥从Base64加密字符串中解密
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="privateKeyPem"></param>
        /// <param name="cipherText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string DecryptFromPkcs8Pem(this IRSAEncryptionService rsaEncryptionService, string privateKeyPem, string cipherText, Encoding encoding = null)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKeyPkcs8Pem(privateKeyPem);
            return rsaEncryptionService.Decrypt(privateKeyParam, cipherText, encoding);
        }

        /// <summary>
        /// RSA加签为Base64
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="privateKeyParam"></param>
        /// <param name="data"></param>
        /// <param name="algorithm">SHA1WITHRSA,SHA256WITHRSA,SHA384WITHRSA,SHA512WITHRSA</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Sign(this IRSAEncryptionService rsaEncryptionService, AsymmetricKeyParameter privateKeyParam, string data, string algorithm = "SHA256WITHRSA", Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var d = encoding.GetBytes(data);
            var signBuffer = rsaEncryptionService.Sign(privateKeyParam, d, algorithm);
            return Base64.ToBase64String(signBuffer);
        }

        /// <summary>
        /// RSA 使用字符串私钥加签为Base64
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="privateKey"></param>
        /// <param name="data"></param>
        /// <param name="algorithm"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Sign(this IRSAEncryptionService rsaEncryptionService, string privateKey, string data, string algorithm = "SHA256WITHRSA", Encoding encoding = null)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKey(privateKey);
            return rsaEncryptionService.Sign(privateKeyParam, data, algorithm, encoding);
        }

        /// <summary>
        /// RSA使用Pem私钥加签为Base64
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="privateKeyPem"></param>
        /// <param name="data"></param>
        /// <param name="algorithm"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string SignFromPem(this IRSAEncryptionService rsaEncryptionService, string privateKeyPem, string data, string algorithm = "SHA256WITHRSA", Encoding encoding = null)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKeyPem(privateKeyPem);
            return rsaEncryptionService.Sign(privateKeyParam, data, algorithm, encoding);
        }

        /// <summary>
        /// RSA使用PKCS8格式私钥加签为Base64
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="privateKey"></param>
        /// <param name="data"></param>
        /// <param name="algorithm"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string SignFromPkcs8(this IRSAEncryptionService rsaEncryptionService, string privateKey, string data, string algorithm = "SHA256WITHRSA", Encoding encoding = null)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKeyPkcs8(privateKey);
            return rsaEncryptionService.Sign(privateKeyParam, data, algorithm, encoding);
        }

        /// <summary>
        /// RSA使用PKCS8格式Pem私钥加签为Base64
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="privateKeyPem"></param>
        /// <param name="data"></param>
        /// <param name="algorithm"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string SignFromPkcs8Pem(this IRSAEncryptionService rsaEncryptionService, string privateKeyPem, string data, string algorithm = "SHA256WITHRSA", Encoding encoding = null)
        {
            var privateKeyParam = rsaEncryptionService.ImportPrivateKeyPkcs8Pem(privateKeyPem);
            return rsaEncryptionService.Sign(privateKeyParam, data, algorithm, encoding);
        }

        /// <summary>
        /// RSA从Base64签名中验签
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="publicKeyParam"></param>
        /// <param name="data"></param>
        /// <param name="signature"></param>
        /// <param name="algorithm"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static bool VerifySign(this IRSAEncryptionService rsaEncryptionService, AsymmetricKeyParameter publicKeyParam, string data, string signature, string algorithm = "SHA256WITHRSA", Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var signatureBuffer = Base64.Decode(signature);
            var dataBuffer = encoding.GetBytes(data);
            return rsaEncryptionService.VerifySign(publicKeyParam, dataBuffer, signatureBuffer, algorithm);
        }

        /// <summary>
        /// RSA从Base64签名中验签
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="publicKey"></param>
        /// <param name="data"></param>
        /// <param name="signature"></param>
        /// <param name="algorithm"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static bool VerifySign(this IRSAEncryptionService rsaEncryptionService, string publicKey, string data, string signature, string algorithm = "SHA256WITHRSA", Encoding encoding = null)
        {
            var publicKeyParam = rsaEncryptionService.ImportPublicKey(publicKey);
            return rsaEncryptionService.VerifySign(publicKeyParam, data, signature, algorithm, encoding);
        }

        /// <summary>
        /// RSA使用Pem公钥,从Base64签名中验签
        /// </summary>
        /// <param name="rsaEncryptionService"></param>
        /// <param name="publicKeyPem"></param>
        /// <param name="data"></param>
        /// <param name="signature"></param>
        /// <param name="algorithm"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static bool VerifySignFromPem(this IRSAEncryptionService rsaEncryptionService, string publicKeyPem, string data, string signature, string algorithm = "SHA256WITHRSA", Encoding encoding = null)
        {
            var publicKeyParam = rsaEncryptionService.ImportPublicKeyPem(publicKeyPem);
            return rsaEncryptionService.VerifySign(publicKeyParam, data, signature, algorithm, encoding);
        }
    }
}
