using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;
using Volo.Abp.Validation;

namespace SharpAbp.MinId
{
    /// <summary>
    /// DTO for creating MinId tokens.
    /// Tokens are used to authenticate and authorize access to MinId services for specific business types.
    /// </summary>
    public class CreateMinIdTokenDto : IValidatableObject
    {
        /// <summary>
        /// The business type identifier associated with this token.
        /// Links the token to a specific MinId configuration.
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(MinIdTokenConsts), nameof(MinIdTokenConsts.MaxBizTypeLength))]
        public string BizType { get; set; }

        /// <summary>
        /// The authentication token value.
        /// Used to validate requests for ID generation services.
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(MinIdTokenConsts), nameof(MinIdTokenConsts.MaxTokenLength))]
        [DisableAuditing]
        public string Token { get; set; }

        /// <summary>
        /// A remark or description for this token.
        /// Provides context about the token's purpose or usage.
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(MinIdTokenConsts), nameof(MinIdTokenConsts.MaxRemarkLength))]
        public string Remark { get; set; }

        /// <summary>
        /// Validates the DTO properties.
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
                yield return new ValidationResult("Invalid Token. Must be 3-40 characters, containing only alphanumeric characters.");
            }

            yield break;
        }
    }
}
