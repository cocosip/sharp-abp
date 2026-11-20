using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;
using Volo.Abp.Validation;

namespace SharpAbp.MinId;

public class FindMinIdTokenDto
{
    [Required]
    [DynamicStringLength(typeof(MinIdTokenConsts), nameof(MinIdTokenConsts.MaxBizTypeLength))]
    public string BizType { get; set; }

    [Required]
    [DynamicStringLength(typeof(MinIdTokenConsts), nameof(MinIdTokenConsts.MaxTokenLength))]
    [DisableAuditing]
    public string Token { get; set; }
}
