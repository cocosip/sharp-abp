using System.ComponentModel.DataAnnotations;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.Identity
{
    public class CreateOrUpdateIdentityRoleClaimDto
    {
        //[Required]
        //public Guid? RoleId { get; set; }

        [Required]
        [DynamicStringLength(typeof(IdentityRoleClaimConsts), nameof(IdentityRoleClaimConsts.MaxClaimTypeLength))]
        public string ClaimType { get; set; }

        [Required]
        [DynamicStringLength(typeof(IdentityRoleClaimConsts), nameof(IdentityRoleClaimConsts.MaxClaimValueLength))]
        public string Value { get; set; }
    }
}
