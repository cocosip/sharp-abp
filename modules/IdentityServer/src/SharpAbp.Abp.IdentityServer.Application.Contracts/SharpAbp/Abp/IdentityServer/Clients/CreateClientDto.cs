using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class CreateClientDto
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

        public bool RequireConsent { get; set; }

        public string LogoutUrl { get; set; }

        public string CallbackUrl { get; set; }

        public List<CreateOrUpdateClientScopeDto> AllowedScopes { get; set; }
        public List<CreateOrUpdateClientSecretDto> ClientSecrets { get; set; }

        public CreateClientDto()
        {
            AllowedScopes = new List<CreateOrUpdateClientScopeDto>();
            ClientSecrets = new List<CreateOrUpdateClientSecretDto>();
        }
    }
}
