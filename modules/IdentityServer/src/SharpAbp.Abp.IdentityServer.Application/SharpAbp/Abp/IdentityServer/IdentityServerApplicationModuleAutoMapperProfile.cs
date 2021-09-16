using AutoMapper;
using SharpAbp.Abp.IdentityServer.ApiResources;
using SharpAbp.Abp.IdentityServer.ApiScopes;
using SharpAbp.Abp.IdentityServer.Clients;
using SharpAbp.Abp.IdentityServer.IdentityResources;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.IdentityServer.ApiScopes;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.IdentityServer.IdentityResources;

namespace SharpAbp.Abp.IdentityServer
{
    public class IdentityServerApplicationModuleAutoMapperProfile : Profile
    {
        public IdentityServerApplicationModuleAutoMapperProfile()
        {
            CreateMap<Client, ClientDto>();
            CreateMap<ClientScope, ClientScopeDto>();
            CreateMap<ClientSecret, ClientSecretDto>();
            CreateMap<ClientGrantType, ClientGrantTypeDto>();
            CreateMap<ClientCorsOrigin, ClientCorsOriginDto>();
            CreateMap<ClientRedirectUri, ClientRedirectUriDto>();
            CreateMap<ClientPostLogoutRedirectUri, ClientPostLogoutRedirectUriDto>();
            CreateMap<ClientIdPRestriction, ClientIdPRestrictionDto>();
            CreateMap<ClientClaim, ClientClaimDto>();
            CreateMap<ClientProperty, ClientPropertyDto>();

            CreateMap<IdentityResource, IdentityResourceDto>();
            CreateMap<IdentityResourceClaim, IdentityResourceClaimDto>();
            CreateMap<IdentityResourceProperty, IdentityResourcePropertyDto>();

            CreateMap<ApiScope, ApiScopeDto>();
            CreateMap<ApiScopeClaim, ApiScopeClaimDto>();
            CreateMap<ApiScopeProperty, ApiScopePropertyDto>();

            CreateMap<ApiResource, ApiResourceDto>();
            CreateMap<ApiResourceClaim, ApiResourceClaimDto>();
            CreateMap<ApiResourceProperty, ApiResourcePropertyDto>();
            CreateMap<ApiResourceScope, ApiResourceScopeDto>();
            CreateMap<ApiResourceSecret, ApiResourceSecretDto>();
        }
    }
}
