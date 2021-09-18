using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.Identity
{
    public class OrganizationUnitRoleDto : CreationAuditedEntityDto<Guid>
    {
        /// <summary>
        /// TenantId of this entity.
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Id of the Role.
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Id of the <see cref="OrganizationUnitDto"/>.
        /// </summary>
        public Guid OrganizationUnitId { get; set; }
    }
}
