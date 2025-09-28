using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Data Transfer Object for updating an existing tenant group.
    /// Contains the modifiable properties of a tenant group with validation attributes and concurrency control.
    /// </summary>
    public class UpdateTenantGroupDto
    {
        /// <summary>
        /// Gets or sets the name of the tenant group.
        /// This field is required and must not exceed the maximum length defined in TenantGroupConsts.
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(TenantGroupConsts), nameof(TenantGroupConsts.MaxNameLength))]
        [Display(Name = "TenantGroupName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tenant group is active.
        /// This field is required and determines the operational status of the tenant group.
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the concurrency stamp for optimistic concurrency control.
        /// This value is used to detect concurrent modifications and prevent update conflicts.
        /// </summary>
        public string ConcurrencyStamp { get; set; }
    }
}
