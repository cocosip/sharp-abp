using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Data transfer object for generating multiple SM2 cryptographic credentials.
    /// This class is used to specify parameters for batch generation of SM2 public-private key pairs.
    /// </summary>
    public class GenerateSM2CredsDto : IValidatableObject
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
        /// All generated key pairs will use this same curve.
        /// </remarks>
        [Required(ErrorMessage = "The SM2 curve name is required and cannot be null or empty.")]
        public string Curve { get; set; }

        /// <summary>
        /// Gets or sets the number of SM2 key pairs to generate.
        /// </summary>
        /// <value>
        /// The count of key pairs to generate. Must be between 1 and 100 inclusive.
        /// </value>
        /// <remarks>
        /// This property controls the batch size for key generation operations.
        /// A reasonable limit is enforced to prevent excessive resource consumption.
        /// </remarks>
        [Required(ErrorMessage = "The key pair count is required.")]
        public int Count { get; set; }

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
        /// It validates that the Count property is within the acceptable range (1-100).
        /// </remarks>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Count <= 0)
            {
                yield return new ValidationResult(
                    "The key pair count must be greater than zero.",
                    new[] { nameof(Count) }
                );
            }
            
            if (Count > 100)
            {
                yield return new ValidationResult(
                    "The key pair count cannot exceed 100 to prevent excessive resource consumption.",
                    new[] { nameof(Count) }
                );
            }
        }
    }
}
