using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.Identity
{
    public class CreateOrganizationUnitDto
    {
        /// <summary>
        /// Parent <see cref="OrganizationUnitDto"/> Id.
        /// Null, if this OU is a root.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Display name of this OrganizationUnit.
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(OrganizationUnitConsts), nameof(OrganizationUnitConsts.MaxDisplayNameLength))]
        public string DisplayName { get; set; }
    }
}
