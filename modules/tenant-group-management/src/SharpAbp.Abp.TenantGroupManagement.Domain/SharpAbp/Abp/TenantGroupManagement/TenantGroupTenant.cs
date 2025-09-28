﻿using System;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Represents a tenant that belongs to a tenant group.
    /// </summary>
    public class TenantGroupTenant : Entity<Guid>
    {
        /// <summary>
        /// Gets or sets the ID of the tenant group that this tenant belongs to.
        /// </summary>
        public virtual Guid TenantGroupId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the tenant.
        /// </summary>
        public virtual Guid TenantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantGroupTenant"/> class.
        /// </summary>
        public TenantGroupTenant()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantGroupTenant"/> class with specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier for this entity.</param>
        /// <param name="tenantGroupId">The ID of the tenant group.</param>
        /// <param name="tenantId">The ID of the tenant.</param>
        public TenantGroupTenant(Guid id, Guid tenantGroupId, Guid tenantId) : base(id)
        {
            TenantGroupId = tenantGroupId;
            TenantId = tenantId;
        }
    }
}