﻿using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Data Transfer Object for requesting paged tenant group data with filtering and sorting options.
    /// Extends the base paged request DTO with tenant group specific filtering capabilities.
    /// </summary>
    public class TenantGroupPagedRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Gets or sets the name filter for searching tenant groups.
        /// When specified, only tenant groups containing this value in their name will be returned.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the active status filter for tenant groups.
        /// When null, both active and inactive tenant groups are included.
        /// When true, only active tenant groups are returned.
        /// When false, only inactive tenant groups are returned.
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
