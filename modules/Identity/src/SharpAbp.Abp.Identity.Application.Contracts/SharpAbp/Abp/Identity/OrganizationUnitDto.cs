using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.Identity
{
    public class OrganizationUnitDto : ExtensibleFullAuditedEntityDto<Guid>
    {
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Parent <see cref="OrganizationUnitDto"/> Id.
        /// Null, if this OU is a root.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Hierarchical Code of this organization unit.
        /// Example: "00001.00042.00005".
        /// This is a unique code for an OrganizationUnit.
        /// It's changeable if OU hierarchy is changed.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Display name of this OrganizationUnit.
        /// </summary>
        public string DisplayName { get; set; }

        public List<OrganizationUnitRoleDto> Roles { get; set; }
        
        public OrganizationUnitDto()
        {
            Roles = [];
        }
    }
}
