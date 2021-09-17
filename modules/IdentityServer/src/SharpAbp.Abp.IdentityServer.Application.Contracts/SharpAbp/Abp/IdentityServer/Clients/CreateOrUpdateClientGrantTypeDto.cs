using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class CreateOrUpdateClientGrantTypeDto
    {
        public Guid? ClientId { get; set; }

        [Required]
        [DynamicStringLength(typeof(ClientGrantTypeConsts), nameof(ClientGrantTypeConsts.GrantTypeMaxLength))]
        public string GrantType { get; set; }
    }
}
