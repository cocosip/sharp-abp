using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using Volo.Abp.DependencyInjection;


namespace SharpAbp.Abp.Crypto.RSA
{
    /// <summary>
    /// Provides RSA encryption, decryption, signing, and verification services.
    /// </summary>
    public class RSAEncryptionService : IRSAEncryptionService, ITransientDependency
    {
        /// <summary>
        /// Generates an RSA key pair.
        /// </summary>
        /// <param name="keySize">The key size in bits (e.g., 2048, 4096). Default is 2048.</param>
        /// <param name="rd">Optional: A secure random number generator. If null, a new one will be created.</param>
        /// <returns>An <see cref="AsymmetricCipherKeyPair"/> containing the public and private keys.</returns>
        public virtual AsymmetricCipherKeyPair GenerateRSAKeyPair(int keySize = 2048, SecureRandom? rd = null)
        {
            rd ??= new SecureRandom();
            var generator = new RsaKeyPairGenerator();
            generator.Init(new KeyGenerationParameters(rd, keySize));
            return generator.GenerateKeyPair();
        }

        /// <summary>
        /// Imports an RSA public key from a PEM formatted string.
        /// </summary>
        /// <param name="publicKeyPem">The PEM formatted string containing the public key.</param>
        /// <returns>An <see cref="AsymmetricKeyParameter"/> representing the public key.</returns>
        public virtual AsymmetricKeyParameter ImportPublicKeyPem(string publicKeyPem)
        {
            using var stringReader = new StringReader(publicKeyPem);
            using var pemReader = new PemReader(stringReader);
            return (AsymmetricKeyParameter)pemReader.ReadObject();
        }

        /// <summary>
        /// Imports an RSA private key from a PEM formatted string.
        /// </summary>
        /// <param name="privateKeyPem">The PEM formatted string containing the private key.</param>
        /// <returns>An <see cref="AsymmetricKeyParameter"/> representing the private key.</returns>
        public virtual AsymmetricKeyParameter ImportPrivateKeyPem(string privateKeyPem)
        {
            using var stringReader = new StringReader(privateKeyPem);
            using var pemReader = new PemReader(stringReader);
            var obj = pemReader.ReadObject();
            if (obj is AsymmetricCipherKeyPair keyPair)
            {
                return keyPair.Private;
            }
            else if (obj is AsymmetricKeyParameter keyParam)
            {
                return keyParam;
            }
            throw new ArgumentException("Invalid private key PEM format.");
        }

        /// <summary>
        /// Imports an RSA private key from a PEM formatted string (PKCS#8 format).
        /// </summary>
        /// <param name="privateKeyPem">The PEM formatted string containing the private key.</param>
        /// <returns>An <see cref="AsymmetricKeyParameter"/> representing the private key.</returns>
        public virtual AsymmetricKeyParameter ImportPrivateKeyPkcs8Pem(string privateKeyPem)
        {
            using var stringReader = new StringReader(privateKeyPem);
            using var pemReader = new PemReader(stringReader);
            var obj = pemReader.ReadObject();
            if (obj is AsymmetricCipherKeyPair keyPair)
            {
                return keyPair.Private;
            }
            else if (obj is AsymmetricKeyParameter keyParam)
            {
                return keyParam;
            }
            throw new ArgumentException("Invalid PKCS#8 private key PEM format.");
        }

        /// <summary>
        /// Encrypts data using RSA with the specified public key and padding.
        /// </summary>
        /// <param name="publicKeyParam">The RSA public key parameter.</param>
        /// <param name="plainText">The plain text data to encrypt.</param>
        /// <param name="padding">The padding scheme to use (e.g., "PKCS1Padding", "OAEPPadding").</param>
        /// <returns>The encrypted data as a byte array.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided key is not a public key.</exception>
        public virtual byte[] Encrypt(AsymmetricKeyParameter publicKeyParam, byte[] plainText, string padding)
        {
            if (publicKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not a public key.", nameof(publicKeyParam));
            }

            var cipher = GetCipher(padding);
            cipher.Init(true, publicKeyParam);
            return cipher.ProcessBlock(plainText, 0, plainText.Length);
        }

        /// <summary>
        /// Decrypts data using RSA with the specified private key and padding.
        /// </summary>
        /// <param name="privateKeyParam">The RSA private key parameter.</param>
        /// <param name="cipherText">The encrypted data to decrypt.</param>
        /// <param name="padding">The padding scheme used during encryption (e.g., "PKCS1Padding", "OAEPPadding").</param>
        /// <returns>The decrypted data as a byte array.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided key is not a private key.</exception>
        public virtual byte[] Decrypt(AsymmetricKeyParameter privateKeyParam, byte[] cipherText, string padding)
        {
            if (!privateKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not a private key.", nameof(privateKeyParam));
            }

            var cipher = GetCipher(padding);
            cipher.Init(false, privateKeyParam);
            return cipher.ProcessBlock(cipherText, 0, cipherText.Length);
        }

        /// <summary>
        /// Signs data using RSA with the specified private key and hashing algorithm.
        /// </summary>
        /// <param name="privateKeyParam">The RSA private key parameter.</param>
        /// <param name="data">The data to sign.</param>
        /// <param name="algorithm">The signing algorithm (e.g., "SHA256WITHRSA", "SHA1WITHRSA"). Default is "SHA256WITHRSA".</param>
        /// <returns>The signature as a byte array.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided key is not a private key.</exception>
        public virtual byte[] Sign(AsymmetricKeyParameter privateKeyParam, byte[] data, string algorithm = "SHA256WITHRSA")
        {
            if (!privateKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not a private key.", nameof(privateKeyParam));
            }

            ISigner signer = SignerUtilities.GetSigner(algorithm);
            signer.Init(true, privateKeyParam);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.GenerateSignature();
        }

        /// <summary>
        /// Verifies a signature using RSA with the specified public key and hashing algorithm.
        /// </summary>
        /// <param name="publicKeyParam">The RSA public key parameter.</param>
        /// <param name="data">The original data that was signed.</param>
        /// <param name="signature">The signature to verify.</param>
        /// <param name="algorithm">The signing algorithm used (e.g., "SHA256WITHRSA", "SHA1WITHRSA"). Default is "SHA256WITHRSA".</param>
        /// <returns>True if the signature is valid, false otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided key is not a public key.</exception>
        public virtual bool VerifySign(AsymmetricKeyParameter publicKeyParam, byte[] data, byte[] signature, string algorithm = "SHA256WITHRSA")
        {
            if (publicKeyParam.IsPrivate)
            {
                throw new ArgumentException("AsymmetricKeyParameter is not a public key.", nameof(publicKeyParam));
            }

            ISigner signer = SignerUtilities.GetSigner(algorithm);
            signer.Init(false, publicKeyParam);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(signature);
        }

        /// <summary>
        /// Gets the appropriate <see cref="IAsymmetricBlockCipher"/> based on the specified padding scheme.
        /// </summary>
        /// <param name="padding">The padding scheme name.</param>
        /// <returns>An <see cref="IAsymmetricBlockCipher"/> instance.</returns>
        protected virtual IAsymmetricBlockCipher GetCipher(string padding)
        {
            return padding switch
            {
                RSAPaddingNames.PKCS1Padding => new Pkcs1Encoding(new RsaEngine()),
                RSAPaddingNames.OAEPPadding or RSAPaddingNames.OAEPSHA1Padding => new OaepEncoding(new RsaEngine(), new Sha1Digest()),
                RSAPaddingNames.OAEPSHA256Padding => new OaepEncoding(new RsaEngine(), new Sha256Digest()),
                RSAPaddingNames.OAEPSHA384Padding => new OaepEncoding(new RsaEngine(), new Sha384Digest()),
                RSAPaddingNames.OAEPSHA512Padding => new OaepEncoding(new RsaEngine(), new Sha512Digest()),
                RSAPaddingNames.ISO9796d1Padding => new ISO9796d1Encoding(new RsaEngine()),
                _ => new RsaEngine(), // Default to no padding or raw RSA
            };
        }
    }
}
