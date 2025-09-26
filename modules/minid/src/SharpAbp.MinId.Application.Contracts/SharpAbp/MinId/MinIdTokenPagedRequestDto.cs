﻿using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Data transfer object for paged requests of MinId tokens.
    /// Inherits paging and sorting capabilities from PagedAndSortedResultRequestDto.
    /// </summary>
    public class MinIdTokenPagedRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Gets or sets the business type for filtering MinId tokens.
        /// Used to identify tokens associated with a specific business context.
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// Gets or sets the token value for filtering MinId tokens.
        /// Used to search for specific token values in the system.
        /// </summary>
        public string Token { get; set; }
    }
}