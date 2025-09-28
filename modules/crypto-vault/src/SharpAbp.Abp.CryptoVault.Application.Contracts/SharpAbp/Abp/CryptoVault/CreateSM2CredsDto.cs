﻿using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Data transfer object for creating SM2 cryptographic credentials.
    /// This class is used to encapsulate the required information for creating SM2 public-private key pairs.
    /// </summary>
    public class CreateSM2CredsDto
    {
        /// <summary>
        /// Gets or sets the elliptic curve name used for SM2 key generation.
        /// </summary>
        /// <value>
        /// The curve name that specifies the elliptic curve parameters for SM2 algorithm.
        /// Common values include "sm2p256v1" which is the standard curve for SM2.
        /// </value>
        /// <remarks>
        /// The curve name must conform to the SM2 standard and be supported by the cryptographic provider.
        /// </remarks>
        [Required(ErrorMessage = "The SM2 curve name is required and cannot be null or empty.")]
        [DynamicStringLength(typeof(SM2CredsConsts), nameof(SM2CredsConsts.MaxCurveLength))]
        public string Curve { get; set; }

        /// <summary>
        /// Gets or sets the SM2 public key in the specified format.
        /// </summary>
        /// <value>
        /// The public key string that represents the SM2 public key component.
        /// This should be in a standard format such as PEM, DER, or hexadecimal string.
        /// </value>
        /// <remarks>
        /// The public key is used for encryption and signature verification operations.
        /// The format should be compatible with the SM2 algorithm implementation.
        /// </remarks>
        [Required(ErrorMessage = "The SM2 public key is required and cannot be null or empty.")]
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the SM2 private key in the specified format.
        /// </summary>
        /// <value>
        /// The private key string that represents the SM2 private key component.
        /// This should be in a standard format such as PEM, DER, or hexadecimal string.
        /// </value>
        /// <remarks>
        /// The private key is used for decryption and digital signature operations.
        /// This is sensitive information and should be handled securely.
        /// The format should be compatible with the SM2 algorithm implementation.
        /// </remarks>
        [Required(ErrorMessage = "The SM2 private key is required and cannot be null or empty.")]
        public string PrivateKey { get; set; }
    }
}
