
using Riok.Mapperly.Abstractions;
using SharpAbp.Abp.IdentityServer.ApiResources;
using SharpAbp.Abp.IdentityServer.ApiScopes;
using SharpAbp.Abp.IdentityServer.Clients;
using SharpAbp.Abp.IdentityServer.IdentityResources;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.IdentityServer.ApiScopes;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.IdentityServer.IdentityResources;
using Volo.Abp.Mapperly;

namespace SharpAbp.Abp.IdentityServer
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ClientToClientDtoMapper : MapperBase<Volo.Abp.IdentityServer.Clients.Client, SharpAbp.Abp.IdentityServer.Clients.ClientDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.Clients.ClientDto Map(Volo.Abp.IdentityServer.Clients.Client source);
        public override partial void Map(Volo.Abp.IdentityServer.Clients.Client source, SharpAbp.Abp.IdentityServer.Clients.ClientDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ClientScopeToClientScopeDtoMapper : MapperBase<Volo.Abp.IdentityServer.Clients.ClientScope, SharpAbp.Abp.IdentityServer.Clients.ClientScopeDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.Clients.ClientScopeDto Map(Volo.Abp.IdentityServer.Clients.ClientScope source);
        public override partial void Map(Volo.Abp.IdentityServer.Clients.ClientScope source, SharpAbp.Abp.IdentityServer.Clients.ClientScopeDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ClientSecretToClientSecretDtoMapper : MapperBase<Volo.Abp.IdentityServer.Clients.ClientSecret, SharpAbp.Abp.IdentityServer.Clients.ClientSecretDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.Clients.ClientSecretDto Map(Volo.Abp.IdentityServer.Clients.ClientSecret source);
        public override partial void Map(Volo.Abp.IdentityServer.Clients.ClientSecret source, SharpAbp.Abp.IdentityServer.Clients.ClientSecretDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ClientGrantTypeToClientGrantTypeDtoMapper : MapperBase<Volo.Abp.IdentityServer.Clients.ClientGrantType, SharpAbp.Abp.IdentityServer.Clients.ClientGrantTypeDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.Clients.ClientGrantTypeDto Map(Volo.Abp.IdentityServer.Clients.ClientGrantType source);
        public override partial void Map(Volo.Abp.IdentityServer.Clients.ClientGrantType source, SharpAbp.Abp.IdentityServer.Clients.ClientGrantTypeDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ClientCorsOriginToClientCorsOriginDtoMapper : MapperBase<Volo.Abp.IdentityServer.Clients.ClientCorsOrigin, SharpAbp.Abp.IdentityServer.Clients.ClientCorsOriginDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.Clients.ClientCorsOriginDto Map(Volo.Abp.IdentityServer.Clients.ClientCorsOrigin source);
        public override partial void Map(Volo.Abp.IdentityServer.Clients.ClientCorsOrigin source, SharpAbp.Abp.IdentityServer.Clients.ClientCorsOriginDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ClientRedirectUriToClientRedirectUriDtoMapper : MapperBase<Volo.Abp.IdentityServer.Clients.ClientRedirectUri, SharpAbp.Abp.IdentityServer.Clients.ClientRedirectUriDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.Clients.ClientRedirectUriDto Map(Volo.Abp.IdentityServer.Clients.ClientRedirectUri source);
        public override partial void Map(Volo.Abp.IdentityServer.Clients.ClientRedirectUri source, SharpAbp.Abp.IdentityServer.Clients.ClientRedirectUriDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ClientPostLogoutRedirectUriToClientPostLogoutRedirectUriDtoMapper : MapperBase<Volo.Abp.IdentityServer.Clients.ClientPostLogoutRedirectUri, SharpAbp.Abp.IdentityServer.Clients.ClientPostLogoutRedirectUriDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.Clients.ClientPostLogoutRedirectUriDto Map(Volo.Abp.IdentityServer.Clients.ClientPostLogoutRedirectUri source);
        public override partial void Map(Volo.Abp.IdentityServer.Clients.ClientPostLogoutRedirectUri source, SharpAbp.Abp.IdentityServer.Clients.ClientPostLogoutRedirectUriDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ClientIdPRestrictionToClientIdPRestrictionDtoMapper : MapperBase<Volo.Abp.IdentityServer.Clients.ClientIdPRestriction, SharpAbp.Abp.IdentityServer.Clients.ClientIdPRestrictionDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.Clients.ClientIdPRestrictionDto Map(Volo.Abp.IdentityServer.Clients.ClientIdPRestriction source);
        public override partial void Map(Volo.Abp.IdentityServer.Clients.ClientIdPRestriction source, SharpAbp.Abp.IdentityServer.Clients.ClientIdPRestrictionDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ClientClaimToClientClaimDtoMapper : MapperBase<Volo.Abp.IdentityServer.Clients.ClientClaim, SharpAbp.Abp.IdentityServer.Clients.ClientClaimDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.Clients.ClientClaimDto Map(Volo.Abp.IdentityServer.Clients.ClientClaim source);
        public override partial void Map(Volo.Abp.IdentityServer.Clients.ClientClaim source, SharpAbp.Abp.IdentityServer.Clients.ClientClaimDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ClientPropertyToClientPropertyDtoMapper : MapperBase<Volo.Abp.IdentityServer.Clients.ClientProperty, SharpAbp.Abp.IdentityServer.Clients.ClientPropertyDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.Clients.ClientPropertyDto Map(Volo.Abp.IdentityServer.Clients.ClientProperty source);
        public override partial void Map(Volo.Abp.IdentityServer.Clients.ClientProperty source, SharpAbp.Abp.IdentityServer.Clients.ClientPropertyDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class IdentityResourceToIdentityResourceDtoMapper : MapperBase<Volo.Abp.IdentityServer.IdentityResources.IdentityResource, SharpAbp.Abp.IdentityServer.IdentityResources.IdentityResourceDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.IdentityResources.IdentityResourceDto Map(Volo.Abp.IdentityServer.IdentityResources.IdentityResource source);
        public override partial void Map(Volo.Abp.IdentityServer.IdentityResources.IdentityResource source, SharpAbp.Abp.IdentityServer.IdentityResources.IdentityResourceDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class IdentityResourceClaimToIdentityResourceClaimDtoMapper : MapperBase<Volo.Abp.IdentityServer.IdentityResources.IdentityResourceClaim, SharpAbp.Abp.IdentityServer.IdentityResources.IdentityResourceClaimDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.IdentityResources.IdentityResourceClaimDto Map(Volo.Abp.IdentityServer.IdentityResources.IdentityResourceClaim source);
        public override partial void Map(Volo.Abp.IdentityServer.IdentityResources.IdentityResourceClaim source, SharpAbp.Abp.IdentityServer.IdentityResources.IdentityResourceClaimDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class IdentityResourcePropertyToIdentityResourcePropertyDtoMapper : MapperBase<Volo.Abp.IdentityServer.IdentityResources.IdentityResourceProperty, SharpAbp.Abp.IdentityServer.IdentityResources.IdentityResourcePropertyDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.IdentityResources.IdentityResourcePropertyDto Map(Volo.Abp.IdentityServer.IdentityResources.IdentityResourceProperty source);
        public override partial void Map(Volo.Abp.IdentityServer.IdentityResources.IdentityResourceProperty source, SharpAbp.Abp.IdentityServer.IdentityResources.IdentityResourcePropertyDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ApiScopeToApiScopeDtoMapper : MapperBase<Volo.Abp.IdentityServer.ApiScopes.ApiScope, SharpAbp.Abp.IdentityServer.ApiScopes.ApiScopeDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.ApiScopes.ApiScopeDto Map(Volo.Abp.IdentityServer.ApiScopes.ApiScope source);
        public override partial void Map(Volo.Abp.IdentityServer.ApiScopes.ApiScope source, SharpAbp.Abp.IdentityServer.ApiScopes.ApiScopeDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ApiScopeClaimToApiScopeClaimDtoMapper : MapperBase<Volo.Abp.IdentityServer.ApiScopes.ApiScopeClaim, SharpAbp.Abp.IdentityServer.ApiScopes.ApiScopeClaimDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.ApiScopes.ApiScopeClaimDto Map(Volo.Abp.IdentityServer.ApiScopes.ApiScopeClaim source);
        public override partial void Map(Volo.Abp.IdentityServer.ApiScopes.ApiScopeClaim source, SharpAbp.Abp.IdentityServer.ApiScopes.ApiScopeClaimDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ApiScopePropertyToApiScopePropertyDtoMapper : MapperBase<Volo.Abp.IdentityServer.ApiScopes.ApiScopeProperty, SharpAbp.Abp.IdentityServer.ApiScopes.ApiScopePropertyDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.ApiScopes.ApiScopePropertyDto Map(Volo.Abp.IdentityServer.ApiScopes.ApiScopeProperty source);
        public override partial void Map(Volo.Abp.IdentityServer.ApiScopes.ApiScopeProperty source, SharpAbp.Abp.IdentityServer.ApiScopes.ApiScopePropertyDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ApiResourceToApiResourceDtoMapper : MapperBase<Volo.Abp.IdentityServer.ApiResources.ApiResource, SharpAbp.Abp.IdentityServer.ApiResources.ApiResourceDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.ApiResources.ApiResourceDto Map(Volo.Abp.IdentityServer.ApiResources.ApiResource source);
        public override partial void Map(Volo.Abp.IdentityServer.ApiResources.ApiResource source, SharpAbp.Abp.IdentityServer.ApiResources.ApiResourceDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ApiResourceClaimToApiResourceClaimDtoMapper : MapperBase<Volo.Abp.IdentityServer.ApiResources.ApiResourceClaim, SharpAbp.Abp.IdentityServer.ApiResources.ApiResourceClaimDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.ApiResources.ApiResourceClaimDto Map(Volo.Abp.IdentityServer.ApiResources.ApiResourceClaim source);
        public override partial void Map(Volo.Abp.IdentityServer.ApiResources.ApiResourceClaim source, SharpAbp.Abp.IdentityServer.ApiResources.ApiResourceClaimDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ApiResourcePropertyToApiResourcePropertyDtoMapper : MapperBase<Volo.Abp.IdentityServer.ApiResources.ApiResourceProperty, SharpAbp.Abp.IdentityServer.ApiResources.ApiResourcePropertyDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.ApiResources.ApiResourcePropertyDto Map(Volo.Abp.IdentityServer.ApiResources.ApiResourceProperty source);
        public override partial void Map(Volo.Abp.IdentityServer.ApiResources.ApiResourceProperty source, SharpAbp.Abp.IdentityServer.ApiResources.ApiResourcePropertyDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ApiResourceScopeToApiResourceScopeDtoMapper : MapperBase<Volo.Abp.IdentityServer.ApiResources.ApiResourceScope, SharpAbp.Abp.IdentityServer.ApiResources.ApiResourceScopeDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.ApiResources.ApiResourceScopeDto Map(Volo.Abp.IdentityServer.ApiResources.ApiResourceScope source);
        public override partial void Map(Volo.Abp.IdentityServer.ApiResources.ApiResourceScope source, SharpAbp.Abp.IdentityServer.ApiResources.ApiResourceScopeDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ApiResourceSecretToApiResourceSecretDtoMapper : MapperBase<Volo.Abp.IdentityServer.ApiResources.ApiResourceSecret, SharpAbp.Abp.IdentityServer.ApiResources.ApiResourceSecretDto>
    {
        public override partial SharpAbp.Abp.IdentityServer.ApiResources.ApiResourceSecretDto Map(Volo.Abp.IdentityServer.ApiResources.ApiResourceSecret source);
        public override partial void Map(Volo.Abp.IdentityServer.ApiResources.ApiResourceSecret source, SharpAbp.Abp.IdentityServer.ApiResources.ApiResourceSecretDto destination);
    }
}
