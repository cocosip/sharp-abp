using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.IdentityServer.ApiScopes;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.ApiScopes
{
    public class UpdateApiScopeDto : ExtensibleEntityDto
    {
        [Required]
        public bool Enabled { get; set; }

        [DynamicStringLength(typeof(ApiScopeConsts), nameof(ApiScopeConsts.NameMaxLength))]
        public string Name { get; set; }

        [DynamicStringLength(typeof(ApiScopeConsts), nameof(ApiScopeConsts.DisplayNameMaxLength))]
        public string DisplayName { get; set; }

        [DynamicStringLength(typeof(ApiScopeConsts), nameof(ApiScopeConsts.DescriptionMaxLength))]
        public string Description { get; set; }

        [Required]
        public bool Required { get; set; }

        [Required]
        public bool Emphasize { get; set; }

        [Required]
        public bool ShowInDiscoveryDocument { get; set; }

        public List<CreateOrUpdateApiScopeClaimDto> UserClaims { get; set; }

        public List<CreateOrUpdateApiScopePropertyDto> Properties { get; set; }
        
        public UpdateApiScopeDto()
        {
            UserClaims = new List<CreateOrUpdateApiScopeClaimDto>();
            Properties = new List<CreateOrUpdateApiScopePropertyDto>();
        }
    }
}
