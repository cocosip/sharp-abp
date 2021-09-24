using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.Identity
{
    public class UpdateIdentityClaimTypeDto : ExtensibleEntityDto
    {
        //[Required]
        //[DynamicStringLength(typeof(IdentityClaimTypeConsts), nameof(IdentityClaimTypeConsts.MaxNameLength))]
        //public string Name { get; set; }

        public bool Required { get; set; }

        public bool IsStatic { get; set; }

        [DynamicStringLength(typeof(IdentityClaimTypeConsts), nameof(IdentityClaimTypeConsts.MaxRegexLength))]
        public string Regex { get; set; }

        [DynamicStringLength(typeof(IdentityClaimTypeConsts), nameof(IdentityClaimTypeConsts.MaxRegexDescriptionLength))]
        public string RegexDescription { get; set; }

        [DynamicStringLength(typeof(IdentityClaimTypeConsts), nameof(IdentityClaimTypeConsts.MaxDescriptionLength))]
        public string Description { get; set; }

        //public IdentityClaimValueType ValueType { get; set; } = IdentityClaimValueType.String;
    }
}
