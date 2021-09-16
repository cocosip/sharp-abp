using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.ApiResources
{
    public class UpdateApiResourceDto : ExtensibleEntityDto<Guid>
    {
        /// <summary>
        /// Name
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(ApiResourceConsts), nameof(ApiResourceConsts.NameMaxLength))]
        public string Name { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        [DynamicStringLength(typeof(ApiResourceConsts), nameof(ApiResourceConsts.DisplayNameMaxLength))]
        public string DisplayName { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [DynamicStringLength(typeof(ApiResourceConsts), nameof(ApiResourceConsts.DescriptionMaxLength))]
        public string Description { get; set; }

        [Required]
        public bool Enabled { get; set; }

        /// <summary>
        /// AllowedAccessTokenSigningAlgorithms
        /// </summary>
        [DynamicStringLength(typeof(ApiResourceConsts), nameof(ApiResourceConsts.AllowedAccessTokenSigningAlgorithmsMaxLength))]
        public string AllowedAccessTokenSigningAlgorithms { get; set; }

        [Required]
        public bool ShowInDiscoveryDocument { get; set; }

        public List<CreateOrUpdateApiResourceClaimDto> UserClaims { get; set; }

        public List<CreateOrUpdateApiResourceScopeDto> Scopes { get; set; }

        public List<CreateOrUpdateApiResourceSecretDto> Secrets { get; set; }

        public List<CreateOrUpdateApiResourcePropertyDto> Properties { get; set; }

        public UpdateApiResourceDto()
        {
            UserClaims = new List<CreateOrUpdateApiResourceClaimDto>();
            Scopes = new List<CreateOrUpdateApiResourceScopeDto>();
            Secrets = new List<CreateOrUpdateApiResourceSecretDto>();
            Properties = new List<CreateOrUpdateApiResourcePropertyDto>();
        }
    }
}
