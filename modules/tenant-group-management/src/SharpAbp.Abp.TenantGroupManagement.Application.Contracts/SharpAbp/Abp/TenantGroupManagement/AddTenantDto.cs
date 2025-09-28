﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Data Transfer Object for adding a tenant to a tenant group.
    /// Contains the tenant identifier required to establish the tenant group membership.
    /// </summary>
    public class AddTenantDto
    {
        /// <summary>
        /// Gets or sets the identifier of the tenant to be added to the tenant group.
        /// This field is required and must be a valid tenant identifier.
        /// </summary>
        [Required]
        public Guid TenantId { get; set; }
    }
}
