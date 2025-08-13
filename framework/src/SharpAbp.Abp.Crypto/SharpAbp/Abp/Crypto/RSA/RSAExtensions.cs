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
    /// <summary>
    /// Provides extension methods for RSA key parameters and key pairs.
    /// </summary>
    public static class RSAExtensions
    {
        /// <summary>
        /// Exports the public key to a Base64 encoded string (ASN.1 DER format).
        /// </summary>
        /// <param name="publicKeyParam">The public key parameter.</param>
        /// <returns>A Base64 encoded string representing the public key.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided key is a private key.</exception>
        public static string ExportPublicKey(this AsymmetricKeyParameter publicKeyParam)
        {
            if (publicKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not a public key.");
            }
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKeyParam);
            return Base64.ToBase64String(publicKeyInfo.GetEncoded());
        }

        /// <summary>
        /// Exports the public key to a PEM formatted string (PKCS#1 format).
        /// </summary>
        /// <param name="publicKeyParam">The public key parameter.</param>
        /// <returns>A PEM formatted string representing the public key.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided key is a private key.</exception>
        public static string ExportPublicKeyToPem(this AsymmetricKeyParameter publicKeyParam)
        {
            if (publicKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not a public key.");
            }
            using var stringWriter = new StringWriter();
            var pemWriter = new PemWriter(stringWriter);
            pemWriter.WriteObject(publicKeyParam);
            pemWriter.Writer.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Exports the private key to a Base64 encoded string (PKCS#1 raw format).
        /// </summary>
        /// <param name="privateKeyParam">The private key parameter.</param>
        /// <returns>A Base64 encoded string representing the private key.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided key is not a private key.</exception>
        public static string ExportPrivateKey(this AsymmetricKeyParameter privateKeyParam)
        {
            if (!privateKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not a private key.");
            }

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
        /// Exports the private key to a PEM formatted string (PKCS#1 format).
        /// </summary>
        /// <param name="privateKeyParam">The private key parameter.</param>
        /// <returns>A PEM formatted string representing the private key.</returns>
        public static string ExportPrivateKeyToPem(this AsymmetricKeyParameter privateKeyParam)
        {
            using var stringWriter = new StringWriter();
            var pemWriter = new PemWriter(stringWriter);
            pemWriter.WriteObject(privateKeyParam);
            pemWriter.Writer.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Exports the private key to a Base64 encoded string (PKCS#8 format).
        /// </summary>
        /// <param name="privateKeyParam">The private key parameter.</param>
        /// <returns>A Base64 encoded string representing the private key in PKCS#8 format.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided key is not a private key.</exception>
        public static string ExportPrivateKeyPkcs8(this AsymmetricKeyParameter privateKeyParam)
        {
            if (!privateKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not a private key.");
            }
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParam);
            return Base64.ToBase64String(privateKeyInfo.GetDerEncoded());
        }

        /// <summary>
        /// Exports the private key to a PEM formatted string (PKCS#8 format).
        /// </summary>
        /// <param name="privateKeyParam">The private key parameter.</param>
        /// <returns>A PEM formatted string representing the private key in PKCS#8 format.</returns>
        public static string ExportPrivateKeyPkcs8ToPem(this AsymmetricKeyParameter privateKeyParam)
        {
            using var stringWriter = new StringWriter();
            var pemWriter = new PemWriter(stringWriter);
            var gen = new Pkcs8Generator(privateKeyParam);
            pemWriter.WriteObject(gen);
            pemWriter.Writer.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Exports the public key from an RSA key pair to a Base64 encoded string (PKCS#1 raw format).
        /// </summary>
        /// <param name="keyPair">The RSA key pair.</param>
        /// <returns>A Base64 encoded string representing the public key.</returns>
        public static string ExportPublicKey(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Public.ExportPublicKey();
        }

        /// <summary>
        /// Exports the public key from an RSA key pair to a PEM formatted string (PKCS#1 format).
        /// </summary>
        /// <param name="keyPair">The RSA key pair.</param>
        /// <returns>A PEM formatted string representing the public key.</returns>
        public static string ExportPublicKeyToPem(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Public.ExportPublicKeyToPem();
        }

        /// <summary>
        /// Exports the private key from an RSA key pair to a Base64 encoded string (PKCS#1 raw format).
        /// </summary>
        /// <param name="keyPair">The RSA key pair.</param>
        /// <returns>A Base64 encoded string representing the private key.</returns>
        public static string ExportPrivateKey(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Private.ExportPrivateKey();
        }

        /// <summary>
        /// Exports the private key from an RSA key pair to a PEM formatted string (PKCS#1 raw format).
        /// </summary>
        /// <param name="keyPair">The RSA key pair.</param>
        /// <returns>A PEM formatted string representing the private key.</returns>
        public static string ExportPrivateKeyToPem(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Private.ExportPrivateKeyToPem();
        }

        /// <summary>
        /// Exports the private key from an RSA key pair to a Base64 encoded string (PKCS#8 format).
        /// </summary>
        /// <param name="keyPair">The RSA key pair.</param>
        /// <returns>A Base64 encoded string representing the private key in PKCS#8 format.</returns>
        public static string ExportPrivateKeyPkcs8(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Private.ExportPrivateKeyPkcs8();
        }

        /// <summary>
        /// Exports the private key from an RSA key pair to a PEM formatted string (PKCS#8 format).
        /// </summary>
        /// <param name="keyPair">The RSA key pair.</param>
        /// <returns>A PEM formatted string representing the private key in PKCS#8 format.</returns>
        public static string ExportPrivateKeyPkcs8ToPem(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Private.ExportPrivateKeyPkcs8ToPem();
        }

        /// <summary>
        /// Derives the public key parameters from the given private key parameters.
        /// </summary>
        /// <param name="privateKeyParam">The private key parameter.</param>
        /// <returns>An <see cref="AsymmetricKeyParameter"/> representing the public key.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided key is not a private key.</exception>
        public static AsymmetricKeyParameter GetPublic(this AsymmetricKeyParameter privateKeyParam)
        {
            if (!privateKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not private.");
            }

            var p = (RsaPrivateCrtKeyParameters)privateKeyParam;
            var publicKeyParam = new RsaKeyParameters(false, p.Modulus, p.PublicExponent);
            return publicKeyParam;
        }
    }
}
