using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.IdentityServer.IdentityResources;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.IdentityResources
{
    public class UpdateIdentityResourceDto : ExtensibleEntityDto
    {
        [Required]
        [DynamicStringLength(typeof(IdentityResourceConsts), nameof(IdentityResourceConsts.NameMaxLength))]
        public string Name { get; set; }

        [DynamicStringLength(typeof(IdentityResourceConsts), nameof(IdentityResourceConsts.DisplayNameMaxLength))]
        public string DisplayName { get; set; }

        [DynamicStringLength(typeof(IdentityResourceConsts), nameof(IdentityResourceConsts.DescriptionMaxLength))]
        public string Description { get; set; }

        [Required]
        public bool Enabled { get; set; }

        [Required]
        public bool Required { get; set; }

        [Required]
        public bool Emphasize { get; set; }

        [Required]
        public bool ShowInDiscoveryDocument { get; set; }

        public List<CreateOrUpdateIdentityResourceClaimDto> UserClaims { get; set; }
        public List<CreateOrUpdateIdentityResourcePropertyDto> Properties { get; set; }

        public UpdateIdentityResourceDto()
        {
            UserClaims = new List<CreateOrUpdateIdentityResourceClaimDto>();
            Properties = new List<CreateOrUpdateIdentityResourcePropertyDto>();
        }
    }
}
