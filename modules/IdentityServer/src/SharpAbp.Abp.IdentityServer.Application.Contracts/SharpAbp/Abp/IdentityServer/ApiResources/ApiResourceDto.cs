using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.ApiResources
{
    public class ApiResourceDto : ExtensibleFullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; }

        public string AllowedAccessTokenSigningAlgorithms { get; set; }

        public bool ShowInDiscoveryDocument { get; set; }

        public List<ApiResourceSecretDto> Secrets { get; set; }

        public List<ApiResourceScopeDto> Scopes { get; set; }

        public List<ApiResourceClaimDto> UserClaims { get; set; }

        public List<ApiResourcePropertyDto> Properties { get; set; }

        public ApiResourceDto()
        {
            Secrets = new List<ApiResourceSecretDto>();
            Scopes = new List<ApiResourceScopeDto>();
            UserClaims = new List<ApiResourceClaimDto>();
            Properties = new List<ApiResourcePropertyDto>();
        }
    }
}
