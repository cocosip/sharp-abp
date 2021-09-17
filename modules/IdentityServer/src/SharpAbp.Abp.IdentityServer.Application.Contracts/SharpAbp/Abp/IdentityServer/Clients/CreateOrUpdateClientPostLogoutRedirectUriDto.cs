using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class CreateOrUpdateClientPostLogoutRedirectUriDto
    {
        public Guid? ClientId { get; set; }

        [Required]
        [DynamicStringLength(typeof(ClientPostLogoutRedirectUriConsts), nameof(ClientPostLogoutRedirectUriConsts.PostLogoutRedirectUriMaxLength))]
        public string PostLogoutRedirectUri { get; set; }
    }
}
