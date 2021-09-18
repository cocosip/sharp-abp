using System.ComponentModel.DataAnnotations;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.Identity
{
    public class UpdateOrganizationUnitDto
    {
        /// <summary>
        /// Display name of this OrganizationUnit.
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(OrganizationUnitConsts), nameof(OrganizationUnitConsts.MaxDisplayNameLength))]
        public string DisplayName { get; set; }
    }
}
