using System.ComponentModel.DataAnnotations;
using Volo.Abp.TenantManagement;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class CreateHybridMapTenantDto : TenantCreateOrUpdateDtoBase
    {
        [EmailAddress]
        [MaxLength(256)]
        [Required]
        public virtual string AdminEmailAddress { get; set; }

        [MaxLength(128)]
        [Required]
        public virtual string AdminPassword { get; set; }

        [Required]
        [DynamicStringLength(typeof(MapTenantConsts), nameof(MapTenantConsts.MaxCodeLength))]
        public string Code { get; set; }

        [Required]
        [DynamicStringLength(typeof(MapTenantConsts), nameof(MapTenantConsts.MaxMapCodeLength))]
        public string MapCode { get; set; }

    }
}