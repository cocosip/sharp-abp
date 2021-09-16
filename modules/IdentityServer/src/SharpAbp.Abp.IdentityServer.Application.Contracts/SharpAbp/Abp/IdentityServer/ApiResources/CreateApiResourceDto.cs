using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.ApiResources
{
    public class CreateApiResourceDto
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

        public CreateApiResourceDto()
        {
            UserClaims = new List<CreateOrUpdateApiResourceClaimDto>();
        }

    }
}