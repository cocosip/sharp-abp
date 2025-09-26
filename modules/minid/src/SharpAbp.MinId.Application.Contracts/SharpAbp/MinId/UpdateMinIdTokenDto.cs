﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Data transfer object for updating MinId authentication tokens.
    /// This DTO contains all necessary parameters for managing authentication tokens
    /// that are used to authorize access to MinId services for specific business types.
    /// </summary>
    public class UpdateMinIdTokenDto : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the business type identifier associated with this token.
        /// This identifier links the token to a specific business context.
        /// Must be 3-32 characters, containing only alphanumeric characters.
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(MinIdTokenConsts), nameof(MinIdTokenConsts.MaxBizTypeLength))]
        public string BizType { get; set; }

        /// <summary>
        /// Gets or sets the authentication token value.
        /// This token is used for authorizing access to MinId generation services.
        /// Must be 3-40 characters, containing only alphanumeric characters.
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(MinIdTokenConsts), nameof(MinIdTokenConsts.MaxTokenLength))]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets an optional remark or description for this token.
        /// This can be used for administrative purposes to describe the token's purpose.
        /// Maximum length is 256 characters.
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(MinIdTokenConsts), nameof(MinIdTokenConsts.MaxRemarkLength))]
        public string Remark { get; set; }

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

            if (!MinIdUtil.IsToken(Token))
            {
                yield return new ValidationResult($"Invalid Token '{Token}'. Must be 3-40 characters, containing only alphanumeric characters.");
            }

            yield break;
        }
    }
}