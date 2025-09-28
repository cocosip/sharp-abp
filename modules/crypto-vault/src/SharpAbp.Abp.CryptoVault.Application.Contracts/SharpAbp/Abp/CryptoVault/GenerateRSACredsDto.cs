using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Data Transfer Object for generating multiple RSA cryptographic credential pairs.
    /// Provides validation rules for RSA key generation parameters including key size constraints
    /// and count limitations to ensure system performance and security compliance.
    /// Used in batch RSA key generation scenarios for cryptographic operations.
    /// </summary>
    public class GenerateRSACredsDto : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the RSA key size in bits for the generated key pairs.
        /// Must be one of the standard RSA key sizes: 1024, 2048, 3072, or 4096 bits.
        /// Larger key sizes provide better security but require more computational resources.
        /// </summary>
        /// <value>The RSA key size in bits. Common values are 2048 (recommended minimum) and 4096 (high security).</value>
        [Required]
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the number of RSA credential pairs to generate in this batch operation.
        /// Must be between 1 and 100 to balance system performance with operational efficiency.
        /// Higher counts may impact system performance during key generation.
        /// </summary>
        /// <value>The number of RSA key pairs to generate. Range: 1-100.</value>
        [Required]
        public int Count { get; set; }

        /// <summary>
        /// Validates the RSA credential generation parameters to ensure they meet security and performance requirements.
        /// Performs comprehensive validation of key size standards and count limitations.
        /// </summary>
        /// <param name="validationContext">The validation context containing additional information about the validation operation.</param>
        /// <returns>A collection of validation results indicating any validation errors found.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Size != 1024 && Size != 2048 && Size != 3072 && Size != 4096)
            {
                yield return new ValidationResult(
                    "RSA key size must be one of the standard sizes: 1024, 2048, 3072, or 4096 bits. " +
                    "For security compliance, 2048 bits is the recommended minimum size.",
                    new[] { nameof(Size) });
            }

            if (Count <= 0 || Count > 100)
            {
                yield return new ValidationResult(
                    "The number of RSA credential pairs to generate must be between 1 and 100. " +
                    "This limitation ensures optimal system performance during batch key generation operations.",
                    new[] { nameof(Count) });
            }
        }
    }
}
