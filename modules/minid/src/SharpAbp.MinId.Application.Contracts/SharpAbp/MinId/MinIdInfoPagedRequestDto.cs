﻿using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Data transfer object for paged requests of MinId information.
    /// Inherits from PagedAndSortedResultRequestDto to provide pagination and sorting capabilities.
    /// Used when querying MinId information with filtering options.
    /// </summary>
    public class MinIdInfoPagedRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Gets or sets the business type identifier for filtering.
        /// Used to filter MinId information records by their business type.
        /// Supports partial matching in search operations.
        /// </summary>
        public string BizType { get; set; }
    }
}