﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Data transfer object for creating RSA cryptographic credentials.
    /// This class is used to encapsulate the required information for creating RSA public-private key pairs.
    /// </summary>
    public class CreateRSACredsDto : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the RSA key size in bits.
        /// </summary>
        /// <value>
        /// The size of the RSA key in bits. Supported values are 1024, 2048, 3072, and 4096.
        /// </value>
        /// <remarks>
        /// The key size determines the strength of the encryption. Larger key sizes provide better security
        /// but require more computational resources. For security reasons, keys smaller than 2048 bits
        /// are generally not recommended for new applications.
        /// </remarks>
        [Required(ErrorMessage = "The RSA key size is required.")]
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the RSA public key in the specified format.
        /// </summary>
        /// <value>
        /// The public key string that represents the RSA public key component.
        /// This should be in a standard format such as PEM, DER, or XML.
        /// </value>
        /// <remarks>
        /// The public key is used for encryption and signature verification operations.
        /// The format should be compatible with the RSA algorithm implementation.
        /// </remarks>
        [Required(ErrorMessage = "The RSA public key is required and cannot be null or empty.")]
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the RSA private key in the specified format.
        /// </summary>
        /// <value>
        /// The private key string that represents the RSA private key component.
        /// This should be in a standard format such as PEM, DER, or XML.
        /// </value>
        /// <remarks>
        /// The private key is used for decryption and digital signature operations.
        /// This is sensitive information and should be handled securely.
        /// The format should be compatible with the RSA algorithm implementation.
        /// </remarks>
        [Required(ErrorMessage = "The RSA private key is required and cannot be null or empty.")]
        public string PrivateKey { get; set; }

        /// <summary>
        /// Validates the current instance and returns validation results.
        /// </summary>
        /// <param name="validationContext">The validation context that provides additional information about the validation operation.</param>
        /// <returns>
        /// A collection of <see cref="ValidationResult"/> objects that represent any validation errors found.
        /// An empty collection indicates that validation was successful.
        /// </returns>
        /// <remarks>
        /// This method performs custom validation logic beyond what can be expressed through data annotations.
        /// It validates that the Size property contains one of the supported RSA key sizes.
        /// </remarks>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Size != 1024 && Size != 2048 && Size != 3072 && Size != 4096)
            {
                yield return new ValidationResult(
                    "The RSA key size must be one of the following values: 1024, 2048, 3072, or 4096 bits.",
                    new[] { nameof(Size) }
                );
            }
        }
    }
}
