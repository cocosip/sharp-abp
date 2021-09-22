using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class UpdateClientDto : ExtensibleEntityDto
    {
        /*
         * Detail
         */

        [Required]
        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.ClientNameMaxLength))]
        public string ClientName { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.DescriptionMaxLength))]
        public string Description { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.ClientUriMaxLength))]
        public string ClientUri { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.LogoUriMaxLength))]
        public string LogoUri { get; set; }
        public bool RequireConsent { get; set; }
        public bool RequireRequestObject { get; set; }
        public bool AllowRememberConsent { get; set; }
        public bool Enabled { get; set; }
        public bool AllowOfflineAccess { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.FrontChannelLogoutUriMaxLength))]
        public string FrontChannelLogoutUri { get; set; }
        public bool FrontChannelLogoutSessionRequired { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.BackChannelLogoutUriMaxLength))]
        public string BackChannelLogoutUri { get; set; }

        public bool BackChannelLogoutSessionRequired { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.AllowedIdentityTokenSigningAlgorithms))]
        public string AllowedIdentityTokenSigningAlgorithms { get; set; }

        /*
         * Token
         */
        [Required]
        public int AccessTokenLifetime { get; set; }

        [Required]
        public int AccessTokenType { get; set; }

        public int? ConsentLifetime { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.PairWiseSubjectSaltMaxLength))]
        public string PairWiseSubjectSalt { get; set; }

        public bool IncludeJwtId { get; set; }
        public int? UserSsoLifetime { get; set; }

        [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.UserCodeTypeMaxLength))]
        public string UserCodeType { get; set; }

        public int DeviceCodeLifetime { get; set; }
        public bool RequirePkce { get; set; }
        public bool RequireClientSecret { get; set; }


        public string[] AllowedCorsOrigins { get; set; }
        public string[] AllowedGrantTypes { get; set; }
        public List<CreateOrUpdateClientClaimDto> Claims { get; set; }
        public List<CreateOrUpdateClientSecretDto> ClientSecrets { get; set; }
        public string[] IdentityProviderRestrictions { get; set; }
        public string[] PostLogoutRedirectUris { get; set; }
        public List<CreateOrUpdateClientPropertyDto> Properties { get; set; }
        public string[] RedirectUris { get; set; }
        public string[] Scopes { get; set; }

        //IdenityResources
        //{displayName: "Your user identifier", name: "openid", left: true}

        //ApiResources
        //{displayName: "AbpCommercialDemo API", name: "AbpCommercialDemo", left: true}

        public UpdateClientDto()
        {
            Claims = new List<CreateOrUpdateClientClaimDto>();
            ClientSecrets = new List<CreateOrUpdateClientSecretDto>();
            Properties = new List<CreateOrUpdateClientPropertyDto>();
        }
    }
}
