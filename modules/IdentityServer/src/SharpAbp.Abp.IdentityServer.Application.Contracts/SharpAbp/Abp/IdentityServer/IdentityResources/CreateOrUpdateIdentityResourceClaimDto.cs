using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.IdentityResources
{
    public class CreateOrUpdateIdentityResourceClaimDto
    {
        [Required]
        [DynamicStringLength(typeof(UserClaimConsts), nameof(UserClaimConsts.TypeMaxLength))]
        public string Type { get; set; }
    }
}
