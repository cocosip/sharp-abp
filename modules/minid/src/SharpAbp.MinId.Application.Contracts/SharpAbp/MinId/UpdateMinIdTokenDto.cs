using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.MinId
{
    public class UpdateMinIdTokenDto : IValidatableObject
    {
        /// <summary>
        /// BizType
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(MinIdTokenConsts), nameof(MinIdTokenConsts.MaxBizTypeLength))]
        public string BizType { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(MinIdTokenConsts), nameof(MinIdTokenConsts.MaxTokenLength))]
        public string Token { get; set; }

        [Required]
        [DynamicStringLength(typeof(MinIdTokenConsts), nameof(MinIdTokenConsts.MaxRemarkLength))]
        public string Remark { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!MinIdUtil.IsBizType(BizType))
            {
                yield return new ValidationResult($"Invalid BizType '{BizType}'.");
            }

            if (!MinIdUtil.IsToken(Token))
            {
                yield return new ValidationResult($"Invalid Token '{Token}'.");
            }

            yield break;
        }
    }
}
