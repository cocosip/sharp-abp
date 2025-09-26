﻿using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Data transfer object for MinId information.
    /// Contains configuration details for generating distributed IDs for a specific business type.
    /// This DTO is used to transfer MinId configuration data between application layers.
    /// </summary>
    public class MinIdInfoDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the business type identifier.
        /// Used to distinguish different business scenarios that require separate ID generation sequences.
        /// Must be between 3 and 32 alphanumeric characters.
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// Gets or sets the maximum ID value.
        /// Represents the upper bound of the ID segment currently allocated for this business type.
        /// When this value is reached, a new segment needs to be allocated.
        /// </summary>
        public long MaxId { get; set; }

        /// <summary>
        /// Gets or sets the step/increment value.
        /// Defines how much to increment the ID each time a new ID is generated.
        /// Must be greater than 0.
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// Gets or sets the delta/preload value.
        /// Determines how many IDs to pre-allocate when the current segment is nearly exhausted.
        /// Must be greater than or equal to 0.
        /// </summary>
        public int Delta { get; set; }

        /// <summary>
        /// Gets or sets the remainder value.
        /// Used for distributing IDs across multiple instances by taking ID % remainder = specific value.
        /// This enables horizontal scaling by ensuring each instance only generates IDs with a specific modulo result.
        /// </summary>
        public int Remainder { get; set; }
    }
}