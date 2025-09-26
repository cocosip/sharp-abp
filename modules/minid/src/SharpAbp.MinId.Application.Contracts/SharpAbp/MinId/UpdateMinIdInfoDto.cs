﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Data transfer object for updating MinId information configuration.
    /// This DTO contains all necessary parameters for configuring ID generation behavior
    /// for a specific business type, including validation rules to ensure data integrity.
    /// </summary>
    public class UpdateMinIdInfoDto : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the business type identifier.
        /// This identifier uniquely identifies a specific business context for ID generation.
        /// Must be 3-32 characters, containing only alphanumeric characters.
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(MinIdInfoConsts), nameof(MinIdInfoConsts.MaxBizTypeLength))]
        public string BizType { get; set; }

        /// <summary>
        /// Gets or sets the maximum ID value that has been allocated.
        /// This represents the upper boundary of IDs that have been safely issued.
        /// Must be a positive number.
        /// </summary>
        [Required]
        public long MaxId { get; set; }

        /// <summary>
        /// Gets or sets the step size for ID allocation.
        /// Determines how many IDs are allocated in each batch to improve performance.
        /// Must be greater than 0.
        /// </summary>
        [Required]
        public int Step { get; set; }

        /// <summary>
        /// Gets or sets the delta value used with remainder for modulo operations.
        /// This helps distribute IDs across different instances in a cluster environment.
        /// Must be greater than or equal to 0.
        /// </summary>
        [Required]
        public int Delta { get; set; }

        /// <summary>
        /// Gets or sets the remainder value used with delta for modulo operations.
        /// This helps distribute IDs across different instances in a cluster environment.
        /// Must be greater than or equal to 0.
        /// </summary>
        [Required]
        public int Remainder { get; set; }

        /// <summary>
        /// Validates the DTO properties according to business rules.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>A collection of validation results.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!MinIdUtil.IsBizType(BizType))
            {
                yield return new ValidationResult($"Invalid BizType '{BizType}'. Must be 3-32 characters, containing only alphanumeric characters.");
            }

            if (Step <= 0)
            {
                yield return new ValidationResult($"Step must be greater than 0. Current value: {Step}.");
            }

            if (Delta < 0)
            {
                yield return new ValidationResult($"Delta must be greater than or equal to 0. Current value: {Delta}.");
            }

            yield break;
        }
    }
}