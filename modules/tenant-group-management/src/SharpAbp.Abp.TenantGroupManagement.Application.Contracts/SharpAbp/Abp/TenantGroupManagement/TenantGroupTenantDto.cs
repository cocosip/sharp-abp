﻿using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Data Transfer Object representing a tenant within a tenant group.
    /// This DTO is used to transfer tenant group membership information between application layers.
    /// </summary>
    public class TenantGroupTenantDto : EntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the identifier of the tenant group that contains this tenant.
        /// </summary>
        public Guid TenantGroupId { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the tenant.
        /// </summary>
        public Guid TenantId { get; set; }
    }
}
