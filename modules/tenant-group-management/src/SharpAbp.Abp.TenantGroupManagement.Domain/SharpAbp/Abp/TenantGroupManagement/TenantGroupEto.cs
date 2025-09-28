﻿using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Event Transfer Object for tenant group information.
    /// </summary>
    public class TenantGroupEto : EntityEto<Guid>
    {
        /// <summary>
        /// Gets or sets the name of the tenant group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tenant group is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the list of tenant IDs associated with this tenant group.
        /// </summary>
        public List<Guid> Tenants { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantGroupEto"/> class.
        /// </summary>
        public TenantGroupEto()
        {
            Tenants = [];
        }
    }
}