using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class CreateOrUpdateClientPropertyDto
    {
        public Guid? ClientId { get; set; }

        [Required]
        [DynamicStringLength(typeof(ClientPropertyConsts), nameof(ClientPropertyConsts.KeyMaxLength))]
        public string Key { get; set; }

        [Required]
        [DynamicStringLength(typeof(ClientPropertyConsts), nameof(ClientPropertyConsts.ValueMaxLength))]
        public string Value { get; set; }
    }
}
