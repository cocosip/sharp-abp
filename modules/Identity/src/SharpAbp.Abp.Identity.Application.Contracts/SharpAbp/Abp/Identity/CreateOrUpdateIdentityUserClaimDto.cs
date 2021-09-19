using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.Identity
{
    public class CreateOrUpdateIdentityUserClaimDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [DynamicStringLength(typeof(IdentityUserClaimConsts), nameof(IdentityUserClaimConsts.MaxClaimTypeLength))]
        public string ClaimType { get; set; }

        [Required]
        [DynamicStringLength(typeof(IdentityUserClaimConsts), nameof(IdentityUserClaimConsts.MaxClaimValueLength))]
        public string ClaimValue { get; set; }
    }
}
