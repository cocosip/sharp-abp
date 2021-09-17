using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class CreateOrUpdateClientClaimDto
    {
        public Guid? ClientId { get; set; }

        [Required]
        [DynamicStringLength(typeof(ClientClaimConsts), nameof(ClientClaimConsts.TypeMaxLength))]
        public string Type { get; set; }

        [Required]
        [DynamicStringLength(typeof(ClientClaimConsts), nameof(ClientClaimConsts.ValueMaxLength))]
        public string Value { get; set; }
    }
}
