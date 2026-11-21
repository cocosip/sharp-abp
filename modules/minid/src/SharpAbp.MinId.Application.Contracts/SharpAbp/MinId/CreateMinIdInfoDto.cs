using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.MinId
{
    /// <summary>
    /// DTO for creating MinId information.
    /// Contains configuration parameters for generating unique IDs within a specific business context.
    /// </summary>
    public class CreateMinIdInfoDto : IValidatableObject
    {
        /// <summary>
        /// The business type identifier for this MinId configuration.
        /// Used to distinguish between different ID sequences for different business purposes.
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(MinIdInfoConsts), nameof(MinIdInfoConsts.MaxBizTypeLength))]
        public string BizType { get; set; }

        /// <summary>
        /// The maximum ID value that has been allocated so far.
        /// Represents the upper boundary of the current ID segment.
        /// </summary>
        [Required]
        public long MaxId { get; set; }

        /// <summary>
        /// The step size for ID allocation.
        /// Defines how many IDs are allocated in each batch to improve performance.
        /// Must be greater than zero.
        /// </summary>
        [Required]
        public int Step { get; set; }

        /// <summary>
        /// The delta value used in ID generation.
        /// Defines the increment between consecutive IDs within a segment.
        /// Must be greater than zero.
        /// </summary>
        [Required]
        public int Delta { get; set; }

        /// <summary>
        /// The remainder value used in ID generation.
        /// Generated IDs satisfy ID % Delta == Remainder.
        /// Must be nonnegative and smaller than both Delta and Step.
        /// </summary>
        [Required]
        public int Remainder { get; set; }

        /// <summary>
        /// Validates the DTO properties.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>A collection of validation results.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!MinIdUtil.IsBizType(BizType))
            {
                yield return new ValidationResult($"Invalid BizType '{BizType}'. The BizType must be between 1 and {MinIdInfoConsts.MaxBizTypeLength} characters long and contain only letters, digits, hyphens, and underscores.");
            }

            if (Step <= 0)
            {
                yield return new ValidationResult($"Invalid Step value '{Step}'. The Step must be greater than 0.");
            }

            if (Delta <= 0)
            {
                yield return new ValidationResult($"Invalid Delta value '{Delta}'. The Delta must be greater than 0.");
            }

            var remainderLimit = Math.Min(Delta, Step);
            if (Remainder < 0 || (remainderLimit > 0 && Remainder >= remainderLimit))
            {
                yield return new ValidationResult($"Invalid Remainder value '{Remainder}'. The Remainder must be nonnegative and smaller than both Delta ({Delta}) and Step ({Step}).");
            }

            yield break;
        }
    }
}
