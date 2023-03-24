using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class CreateTenantGroupDto
    {
        [Required]
        [DynamicStringLength(typeof(TenantGroupConsts), nameof(TenantGroupConsts.MaxNameLength))]
        [Display(Name = "TenantGroupName")]
        public string Name { get; set; }
    }
}
