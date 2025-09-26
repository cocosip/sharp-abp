﻿using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Represents the configuration and state information for MinId generation for a specific business type.
    /// This aggregate root manages the ID allocation parameters including maximum ID, step size,
    /// delta modulo, and remainder for distributed unique ID generation.
    /// </summary>
    public class MinIdInfo : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Gets or sets the business type identifier that this MinId configuration serves.
        /// This is used to differentiate between different ID generation contexts.
        /// </summary>
        public virtual string BizType { get; set; }

        /// <summary>
        /// Gets or sets the current maximum ID that has been allocated.
        /// This value is incremented by the step size during each segment allocation.
        /// </summary>
        public virtual long MaxId { get; set; }

        /// <summary>
        /// Gets or sets the step size for ID allocation.
        /// This determines how many IDs are allocated in each segment.
        /// </summary>
        public virtual int Step { get; set; }

        /// <summary>
        /// Gets or sets the delta value used for modulo operations in distributed scenarios.
        /// This helps ensure ID uniqueness across multiple instances.
        /// </summary>
        public virtual int Delta { get; set; }

        /// <summary>
        /// Gets or sets the remainder value used with delta for modulo operations.
        /// This helps distribute IDs across different instances in a cluster.
        /// </summary>
        public virtual int Remainder { get; set; }

        /// <summary>
        /// Initializes a new instance of the MinIdInfo class.
        /// </summary>
        public MinIdInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MinIdInfo class with specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier for this MinId configuration.</param>
        /// <param name="bizType">The business type identifier.</param>
        /// <param name="maxId">The initial maximum ID value.</param>
        /// <param name="step">The step size for ID allocation.</param>
        /// <param name="delta">The delta value for modulo operations.</param>
        /// <param name="remainder">The remainder value for modulo operations.</param>
        public MinIdInfo(Guid id, string bizType, long maxId, int step, int delta, int remainder)
        {
            Id = id;
            BizType = bizType;
            MaxId = maxId;
            Step = step;
            Delta = delta;
            Remainder = remainder;
        }

        /// <summary>
        /// Updates all configuration parameters for this MinId instance.
        /// </summary>
        /// <param name="bizType">The business type identifier.</param>
        /// <param name="maxId">The maximum ID value.</param>
        /// <param name="step">The step size for ID allocation.</param>
        /// <param name="delta">The delta value for modulo operations.</param>
        /// <param name="remainder">The remainder value for modulo operations.</param>
        public void Update(string bizType, long maxId, int step, int delta, int remainder)
        {
            BizType = bizType;
            MaxId = maxId;
            Step = step;
            Delta = delta;
            Remainder = remainder;
        }

        /// <summary>
        /// Updates only the maximum ID value, typically called during segment allocation.
        /// </summary>
        /// <param name="maxId">The new maximum ID value.</param>
        public void UpdateMaxId(long maxId)
        {
            MaxId = maxId;
        }
    }
}
