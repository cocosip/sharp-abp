using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class UpdateClientDto : ExtensibleEntityDto
    {
        [Required]
        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.ClientIdMaxLength))]
        public string ClientId { get; set; }

        [Required]
        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.ClientNameMaxLength))]
        public string ClientName { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.DescriptionMaxLength))]
        public string Description { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.ClientUriMaxLength))]
        public string ClientUri { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.LogoUriMaxLength))]
        public string LogoUri { get; set; }

        public bool Enabled { get; set; }

        [Required]
        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.ProtocolTypeMaxLength))]
        public string ProtocolType { get; set; }

        public bool RequireClientSecret { get; set; }

        public bool RequireConsent { get; set; }

        public bool AllowRememberConsent { get; set; }

        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        public bool RequirePkce { get; set; }

        public bool AllowPlainTextPkce { get; set; }

        public bool RequireRequestObject { get; set; }

        public bool AllowAccessTokensViaBrowser { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.FrontChannelLogoutUriMaxLength))]
        public string FrontChannelLogoutUri { get; set; }

        public bool FrontChannelLogoutSessionRequired { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.BackChannelLogoutUriMaxLength))]
        public string BackChannelLogoutUri { get; set; }

        public bool BackChannelLogoutSessionRequired { get; set; }

        public bool AllowOfflineAccess { get; set; }

        public int IdentityTokenLifetime { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.AllowedIdentityTokenSigningAlgorithms))]
        public string AllowedIdentityTokenSigningAlgorithms { get; set; }

        public int AccessTokenLifetime { get; set; }

        public int AuthorizationCodeLifetime { get; set; }

        public int? ConsentLifetime { get; set; }

        public int AbsoluteRefreshTokenLifetime { get; set; }

        public int SlidingRefreshTokenLifetime { get; set; }

        public int RefreshTokenUsage { get; set; }

        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        public int RefreshTokenExpiration { get; set; }

        public int AccessTokenType { get; set; }

        public bool EnableLocalLogin { get; set; }

        public bool IncludeJwtId { get; set; }

        public bool AlwaysSendClientClaims { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.ClientClaimsPrefixMaxLength))]
        public string ClientClaimsPrefix { get; set; }

        public string PairWiseSubjectSalt { get; set; }

        public int? UserSsoLifetime { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.UserCodeTypeMaxLength))]
        public string UserCodeType { get; set; }

        public int DeviceCodeLifetime { get; set; }

        public List<CreateOrUpdateClientScopeDto> AllowedScopes { get; set; }
        public List<CreateOrUpdateClientSecretDto> ClientSecrets { get; set; }
        public List<CreateOrUpdateClientGrantTypeDto> AllowedGrantTypes { get; set; }
        public List<CreateOrUpdateClientCorsOriginDto> AllowedCorsOrigins { get; set; }
        public List<CreateOrUpdateClientRedirectUriDto> RedirectUris { get; set; }
        public List<CreateOrUpdateClientPostLogoutRedirectUriDto> PostLogoutRedirectUris { get; set; }
        public List<CreateOrUpdateClientIdPRestrictionDto> IdentityProviderRestrictions { get; set; }
        public List<CreateOrUpdateClientClaimDto> Claims { get; set; }
        public List<CreateOrUpdateClientPropertyDto> Properties { get; set; }

        public UpdateClientDto()
        {
            AllowedScopes = new List<CreateOrUpdateClientScopeDto>();
            ClientSecrets = new List<CreateOrUpdateClientSecretDto>();
            AllowedGrantTypes = new List<CreateOrUpdateClientGrantTypeDto>();
            AllowedCorsOrigins = new List<CreateOrUpdateClientCorsOriginDto>();
            RedirectUris = new List<CreateOrUpdateClientRedirectUriDto>();
            PostLogoutRedirectUris = new List<CreateOrUpdateClientPostLogoutRedirectUriDto>();
            IdentityProviderRestrictions = new List<CreateOrUpdateClientIdPRestrictionDto>();
            Claims = new List<CreateOrUpdateClientClaimDto>();
            Properties = new List<CreateOrUpdateClientPropertyDto>();
        }
    }
}
