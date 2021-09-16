using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer.ApiScopes;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.ApiScopes
{
    public class CreateOrUpdateApiScopePropertyDto
    {
        public Guid? ApiScopeId { get; set; }

        [Required]
        [DynamicStringLength(typeof(ApiScopePropertyConsts), nameof(ApiScopePropertyConsts.KeyMaxLength))]
        public string Key { get; set; }

        [Required]
        [DynamicStringLength(typeof(ApiScopePropertyConsts), nameof(ApiScopePropertyConsts.ValueMaxLength))]
        public string Value { get; set; }
    }
}
