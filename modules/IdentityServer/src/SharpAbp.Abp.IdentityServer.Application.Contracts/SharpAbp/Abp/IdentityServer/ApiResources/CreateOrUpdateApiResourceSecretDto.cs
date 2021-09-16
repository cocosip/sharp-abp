using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.ApiResources
{
    public class CreateOrUpdateApiResourceSecretDto
    {
        public Guid? ApiResourceId { get; set; }

        [Required]
        [DynamicStringLength(typeof(ApiResourceSecretConsts), nameof(ApiResourceSecretConsts.TypeMaxLength))]
        public string Type { get; set; }

        [Required]
        [DynamicStringLength(typeof(ApiResourceSecretConsts), nameof(ApiResourceSecretConsts.ValueMaxLength))]
        public string Value { get; set; }

        [DynamicStringLength(typeof(ApiResourceSecretConsts), nameof(ApiResourceSecretConsts.DescriptionMaxLength))]
        public string Description { get; set; }

        public DateTime? Expiration { get; set; }
    }
}
