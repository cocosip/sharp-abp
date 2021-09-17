using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.ApiResources
{
    public class CreateOrUpdateApiResourceScopeDto
    {
        public Guid? ApiResourceId { get; set; }

        [Required]
        [DynamicStringLength(typeof(ApiResourceScopeConsts), nameof(ApiResourceScopeConsts.ScopeMaxLength))]
        public string Scope { get; set; }
    }
}
