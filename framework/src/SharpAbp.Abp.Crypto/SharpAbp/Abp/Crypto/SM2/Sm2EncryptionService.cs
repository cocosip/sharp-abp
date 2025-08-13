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

namespace SharpAbp.Abp.Crypto.SM2
{
    /// <summary>
    /// Provides SM2 encryption, decryption, signing, and verification services.
    /// </summary>
    public class Sm2EncryptionService : ISm2EncryptionService, ITransientDependency
    {
        protected AbpSm2EncryptionOptions Options { get; }

        public Sm2EncryptionService(IOptions<AbpSm2EncryptionOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Generates an SM2 key pair.
        /// </summary>
        /// <param name="curve">The curve name to use, defaults to Sm2p256v1.</param>
        /// <param name="rd">Optional: A secure random number generator. If null, a new one will be created.</param>
        /// <returns>An <see cref="AsymmetricCipherKeyPair"/> containing the public and private keys.</returns>
        public virtual AsymmetricCipherKeyPair GenerateSm2KeyPair(
            string curve = Sm2EncryptionNames.CurveSm2p256v1,
            SecureRandom? rd = null)
        {
            rd ??= new SecureRandom();
            if (string.IsNullOrWhiteSpace(curve))
            {
                curve = Options.DefaultCurve!;
            }

            var generator = new ECKeyPairGenerator();
            generator.Init(new ECKeyGenerationParameters(new ECDomainParameters(GMNamedCurves.GetByName(curve)), rd));
            return generator.GenerateKeyPair();
        }

        /// <summary>
        /// Encrypts data using the SM2 public key.
        /// </summary>
        /// <param name="publicKey">The public key as a byte array.</param>
        /// <param name="plainText">The plain text data to encrypt.</param>
        /// <param name="curve">The curve name used for encryption. Defaults to Sm2p256v1.</param>
        /// <param name="mode">The encryption mode. Defaults to C1C2C3.</param>
        /// <returns>The encrypted data as a byte array.</returns>
        public virtual byte[] Encrypt(
            byte[] publicKey,
            byte[] plainText,
            string curve = Sm2EncryptionNames.CurveSm2p256v1,
            Mode mode = Mode.C1C2C3)
        {
            if (string.IsNullOrWhiteSpace(curve))
            {
                curve = Options.DefaultCurve!;
            }

            var engine = new SM2Engine(mode);
            var x9Ec = GMNamedCurves.GetByName(curve);
            var p = new ECPublicKeyParameters(x9Ec.Curve.DecodePoint(publicKey), new ECDomainParameters(x9Ec));
            engine.Init(true, new ParametersWithRandom(p));
            var v = engine.ProcessBlock(plainText, 0, plainText.Length);
            return v;
        }

        /// <summary>
        /// Decrypts data using the SM2 private key.
        /// </summary>
        /// <param name="privateKey">The private key as a byte array.</param>
        /// <param name="cipherText">The encrypted data to decrypt.</param>
        /// <param name="curve">The curve name used for decryption. Defaults to Sm2p256v1.</param>
        /// <param name="mode">The encryption mode. Defaults to C1C2C3.</param>
        /// <returns>The decrypted data as a byte array.</returns>
        public virtual byte[] Decrypt(
            byte[] privateKey,
            byte[] cipherText,
            string curve = Sm2EncryptionNames.CurveSm2p256v1,
            Mode mode = Mode.C1C2C3)
        {
            if (string.IsNullOrWhiteSpace(curve))
            {
                curve = Options.DefaultCurve!;
            }

            var engine = new SM2Engine(mode);
            var x9Ec = GMNamedCurves.GetByName(curve);
            var p = new ECPrivateKeyParameters(new BigInteger(privateKey), new ECDomainParameters(x9Ec));
            engine.Init(false, p);

            var v = engine.ProcessBlock(cipherText, 0, cipherText.Length);
            return v;
        }

        /// <summary>
        /// Signs data using the SM2 private key.
        /// </summary>
        /// <param name="privateKey">The private key as a byte array.</param>
        /// <param name="plainText">The plain text data to sign.</param>
        /// <param name="curve">The curve name used for signing. Defaults to Sm2p256v1.</param>
        /// <param name="id">Optional: The ID to use for signing. If null, a default ID will be used.</param>
        /// <returns>The signature as a byte array.</returns>
        public virtual byte[] Sign(
            byte[] privateKey,
            byte[] plainText,
            string curve = Sm2EncryptionNames.CurveSm2p256v1,
            byte[]? id = null)
        {
            if (string.IsNullOrWhiteSpace(curve))
            {
                curve = Options.DefaultCurve!;
            }

            var x9Ec = GMNamedCurves.GetByName(curve);
            var p = new ECPrivateKeyParameters(new BigInteger(privateKey), new ECDomainParameters(x9Ec));

            var signer = new SM2Signer();
            ICipherParameters cp = id != null
                ? new ParametersWithID(new ParametersWithRandom(p), id)
                : new ParametersWithRandom(p);

            signer.Init(true, cp);
            signer.BlockUpdate(plainText, 0, plainText.Length);
            return signer.GenerateSignature();
        }

        /// <summary>
        /// Verifies a signature using the SM2 public key.
        /// </summary>
        /// <param name="publicKey">The public key as a byte array.</param>
        /// <param name="plainText">The original plain text data.</param>
        /// <param name="signature">The signature to verify.</param>
        /// <param name="curve">The curve name used for verification. Defaults to Sm2p256v1.</param>
        /// <param name="id">Optional: The ID used during signing. If null, a default ID will be used.</param>
        /// <returns>True if the signature is valid, false otherwise.</returns>
        public virtual bool VerifySign(
            byte[] publicKey,
            byte[] plainText,
            byte[] signature,
            string curve = Sm2EncryptionNames.CurveSm2p256v1,
            byte[]? id = null)
        {
            if (string.IsNullOrWhiteSpace(curve))
            {
                curve = Options.DefaultCurve!;
            }

            var x9Ec = GMNamedCurves.GetByName(curve);
            var p = new ECPublicKeyParameters(x9Ec.Curve.DecodePoint(publicKey), new ECDomainParameters(x9Ec));
            var signer = new SM2Signer();

            ICipherParameters cp = id != null
                ? new ParametersWithID(p, id)
                : p;
            signer.Init(false, cp);
            signer.BlockUpdate(plainText, 0, plainText.Length);
            return signer.VerifySignature(signature);
        }

        /// <summary>
        /// Converts C1C2C3 encoded ciphertext to C1C3C2 encoded ciphertext.
        /// </summary>
        /// <param name="c1c2c3">The C1C2C3 encoded ciphertext.</param>
        /// <param name="curve">The curve name used for encryption. Defaults to Sm2p256v1.</param>
        /// <returns>The C1C3C2 encoded ciphertext.</returns>
        public virtual byte[] C123ToC132(
            byte[] c1c2c3,
            string curve = Sm2EncryptionNames.CurveSm2p256v1)
        {
            if (string.IsNullOrWhiteSpace(curve))
            {
                curve = Options.DefaultCurve!;
            }

            var x9Ec = GMNamedCurves.GetByName(curve);
            var c1Len = (x9Ec.Curve.FieldSize + 7) / 8 * 2 + 1;
            var c3Len = 32;
            var result = new byte[c1c2c3.Length];
            Array.Copy(c1c2c3, 0, result, 0, c1Len);
            Array.Copy(c1c2c3, c1c2c3.Length - c3Len, result, c1Len, c3Len);
            Array.Copy(c1c2c3, c1Len, result, c1Len + c3Len, c1c2c3.Length - c1Len - c3Len);
            return result;
        }

        /// <summary>
        /// Converts C1C3C2 encoded ciphertext to C1C2C3 encoded ciphertext.
        /// </summary>
        /// <param name="c1c3c2">The C1C3C2 encoded ciphertext.</param>
        /// <param name="curve">The curve name used for encryption. Defaults to Sm2p256v1.</param>
        /// <returns>The C1C2C3 encoded ciphertext.</returns>
        public virtual byte[] C132ToC123(byte[] c1c3c2, string curve = Sm2EncryptionNames.CurveSm2p256v1)
        {
            if (string.IsNullOrWhiteSpace(curve))
            {
                curve = Options.DefaultCurve!;
            }

            var x9Ec = GMNamedCurves.GetByName(curve);
            var c1Len = (x9Ec.Curve.FieldSize + 7) / 8 * 2 + 1;
            var c3Len = 32;
            var result = new byte[c1c3c2.Length];
            Array.Copy(c1c3c2, 0, result, 0, c1Len);
            Array.Copy(c1c3c2, c1Len + c3Len, result, c1Len, c1c3c2.Length - c1Len - c3Len);
            Array.Copy(c1c3c2, c1Len, result, c1c3c2.Length - c3Len, c3Len);
            return result;
        }
    }
}
