using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class CreateOrUpdateClientRedirectUriDto
    {
        public Guid? ClientId { get; set; }

        [Required]
        [DynamicStringLength(typeof(ClientRedirectUriConsts), nameof(ClientRedirectUriConsts.RedirectUriMaxLength))]
        public string RedirectUri { get; set; }
    }
}
