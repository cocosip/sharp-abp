using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class CreateOrUpdateClientSecretDto
    {
        /// <summary>
        /// ClientId
        /// </summary>
        public Guid? ClientId { get; set; }


        [Required]
        [DynamicStringLength(typeof(ClientSecretConsts), nameof(ClientSecretConsts.TypeMaxLength))]
        public string Type { get; set; }

        [Required]
        [DynamicStringLength(typeof(ClientSecretConsts), nameof(ClientSecretConsts.ValueMaxLength))]
        public string Value { get; set; }

        [DynamicStringLength(typeof(ClientSecretConsts), nameof(ClientSecretConsts.DescriptionMaxLength))]
        public string Description { get; set; }

        public DateTime? Expiration { get; set; }
    }
}
