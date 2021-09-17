using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class CreateOrUpdateClientIdPRestrictionDto
    {
        public Guid? ClientId { get; set; }

        [Required]
        [DynamicStringLength(typeof(ClientIdPRestrictionConsts), nameof(ClientIdPRestrictionConsts.ProviderMaxLength))]
        public string Provider { get; set; }
    }
}
