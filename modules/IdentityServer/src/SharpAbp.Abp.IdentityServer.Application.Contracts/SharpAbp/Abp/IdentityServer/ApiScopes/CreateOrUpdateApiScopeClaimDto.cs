using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.ApiScopes
{
    public class CreateOrUpdateApiScopeClaimDto
    {
        [Required]
        [DynamicStringLength(typeof(UserClaimConsts), nameof(UserClaimConsts.TypeMaxLength))]
        public string Type { get; set; }
    }
}
