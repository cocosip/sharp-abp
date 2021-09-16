using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer.ApiResources
{
    public class CreateOrUpdateApiResourcePropertyDto
    {
        public Guid? ApiResourceId { get; set; }

        [Required]
        [DynamicStringLength(typeof(ApiResourcePropertyConsts), nameof(ApiResourcePropertyConsts.KeyMaxLength))]
        public string Key { get; set; }

        [Required]
        [DynamicStringLength(typeof(ApiResourcePropertyConsts), nameof(ApiResourcePropertyConsts.ValueMaxLength))]
        public string Value { get; set; }
    }
}
