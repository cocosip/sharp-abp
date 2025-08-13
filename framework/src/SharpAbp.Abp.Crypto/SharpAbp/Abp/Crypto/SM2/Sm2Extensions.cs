using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using System;

namespace SharpAbp.Abp.Crypto.SM2
{
    /// <summary>
    /// Provides extension methods for SM2 key parameters and key pairs.
    /// </summary>
    public static class Sm2Extensions
    {
        /// <summary>
        /// Exports the SM2 public key parameters to a hexadecimal string.
        /// </summary>
        /// <param name="publicKeyParam">The public key parameter.</param>
        /// <returns>A hexadecimal string representing the public key.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided key is a private key.</exception>
        public static string ExportPublicKey(this AsymmetricKeyParameter publicKeyParam)
        {
            if (publicKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not a public key.");
            }

            var pub = ((ECPublicKeyParameters)publicKeyParam).Q.GetEncoded();
            return Hex.ToHexString(pub);
        }

        /// <summary>
        /// Exports the SM2 private key parameters to a hexadecimal string.
        /// </summary>
        /// <param name="privateKeyParam">The private key parameter.</param>
        /// <returns>A hexadecimal string representing the private key.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided key is not a private key.</exception>
        public static string ExportPrivateKey(this AsymmetricKeyParameter privateKeyParam)
        {
            if (!privateKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not a private key.");
            }

            var priv = ((ECPrivateKeyParameters)privateKeyParam).D.ToByteArray();
            return Hex.ToHexString(priv);
        }

        /// <summary>
        /// Exports the public key from an SM2 key pair to a hexadecimal string.
        /// </summary>
        /// <param name="keyPair">The SM2 key pair.</param>
        /// <returns>A hexadecimal string representing the public key.</returns>
        public static string ExportPublicKey(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Public.ExportPublicKey();
        }

        /// <summary>
        /// Exports the private key from an SM2 key pair to a hexadecimal string.
        /// </summary>
        /// <param name="keyPair">The SM2 key pair.</param>
        /// <returns>A hexadecimal string representing the private key.</returns>
        public static string ExportPrivateKey(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Private.ExportPrivateKey();
        }
    }
}
