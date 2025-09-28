﻿using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Data Transfer Object representing a tenant group with all its associated data.
    /// This DTO is used to transfer complete tenant group information including tenants and connection strings.
    /// </summary>
    public class TenantGroupDto : ExtensibleEntityDto<Guid>, IHasConcurrencyStamp
    {
        /// <summary>
        /// Gets or sets the name of the tenant group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the normalized name of the tenant group.
        /// This is typically used for case-insensitive searches and comparisons.
        /// </summary>
        public string NormalizedName { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the tenant group is active.
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Gets or sets the concurrency stamp for optimistic concurrency control.
        /// This value is used to detect concurrent modifications to the same entity.
        /// </summary>
        public string ConcurrencyStamp { get; set; }
        
        /// <summary>
        /// Gets or sets the list of tenants that belong to this tenant group.
        /// </summary>
        public List<TenantGroupTenantDto> Tenants { get; set; } = [];
        
        /// <summary>
        /// Gets or sets the list of connection strings associated with this tenant group.
        /// </summary>
        public List<TenantGroupConnectionStringDto> ConnectionStrings { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantGroupDto"/> class.
        /// </summary>
        public TenantGroupDto()
        {

        }
    }
}
