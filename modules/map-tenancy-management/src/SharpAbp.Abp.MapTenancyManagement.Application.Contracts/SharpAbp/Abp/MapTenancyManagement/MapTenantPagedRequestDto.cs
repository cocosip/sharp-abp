﻿using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Data transfer object for paginated map tenant requests with filtering and sorting capabilities.
    /// Extends the base paged request functionality with map tenant-specific filter parameters.
    /// </summary>
    public class MapTenantPagedRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Gets or sets the general filter text for searching across multiple fields
        /// </summary>
        /// <value>The filter text to search in tenant names and codes; can be null or empty</value>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the specific tenant identifier for filtering
        /// </summary>
        /// <value>The unique identifier of the tenant to filter by; optional parameter</value>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the tenant name filter
        /// </summary>
        /// <value>The tenant name to filter by; supports partial matching</value>
        public string TenantName { get; set; }

        /// <summary>
        /// Gets or sets the code filter
        /// </summary>
        /// <value>The tenant code to filter by; supports partial matching</value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the map code filter
        /// </summary>
        /// <value>The map code to filter by; supports partial matching</value>
        public string MapCode { get; set; }
    }
}
