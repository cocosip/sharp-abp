﻿using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Data transfer object for requesting paginated file storing containers with filtering options.
    /// </summary>
    public class FileStoringContainerPagedRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Gets or sets the name filter for containers.
        /// When specified, only containers with names containing this value will be returned.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the provider filter for containers.
        /// When specified, only containers using this provider will be returned.
        /// </summary>
        public string Provider { get; set; }

    }
}
