using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Data Transfer Object for creating a new tenant group.
    /// Contains the required information to create a tenant group with validation attributes.
    /// </summary>
    public class CreateTenantGroupDto
    {
        /// <summary>
        /// Gets or sets the name of the tenant group to be created.
        /// This field is required and must not exceed the maximum length defined in TenantGroupConsts.
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(TenantGroupConsts), nameof(TenantGroupConsts.MaxNameLength))]
        [Display(Name = "TenantGroupName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tenant group should be active upon creation.
        /// This field is required and determines the initial active status of the tenant group.
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
    }
}
