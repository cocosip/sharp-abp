using System.ComponentModel.DataAnnotations;
using Volo.Abp.TenantManagement;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Data transfer object for creating a hybrid map tenant with tenant creation and mapping capabilities.
    /// Extends tenant creation functionality to include map tenant properties and administrator setup.
    /// </summary>
    public class CreateHybridMapTenantDto : TenantCreateOrUpdateDtoBase
    {
        /// <summary>
        /// Gets or sets the email address of the tenant administrator
        /// </summary>
        /// <value>The administrator's email address; required field with email format validation and maximum length of 256 characters</value>
        [EmailAddress]
        [MaxLength(256)]
        [Required]
        public virtual string AdminEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the password for the tenant administrator
        /// </summary>
        /// <value>The administrator's password; required field with maximum length of 128 characters</value>
        [MaxLength(128)]
        [Required]
        public virtual string AdminPassword { get; set; }

        /// <summary>
        /// Gets or sets the unique code associated with the tenant
        /// </summary>
        /// <value>The tenant's unique identification code; required field with dynamic length validation</value>
        [Required]
        [DynamicStringLength(typeof(MapTenantConsts), nameof(MapTenantConsts.MaxCodeLength))]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the map code for external system integration
        /// </summary>
        /// <value>The external mapping code; required field with dynamic length validation</value>
        [Required]
        [DynamicStringLength(typeof(MapTenantConsts), nameof(MapTenantConsts.MaxMapCodeLength))]
        public string MapCode { get; set; }
    }
}