﻿﻿﻿﻿﻿﻿﻿using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Domain service for managing MinId information with business logic validation.
    /// This service implements domain-level operations for creating and updating MinId configurations
    /// while enforcing business rules such as business type uniqueness and data consistency.
    /// </summary>
    public class MinIdInfoManager : DomainService, IMinIdInfoManager
    {
        /// <summary>
        /// Gets the repository for MinId information persistence operations.
        /// </summary>
        protected IMinIdInfoRepository MinIdInfoRepository { get; }

        /// <summary>
        /// Initializes a new instance of the MinIdInfoManager with required dependencies.
        /// </summary>
        /// <param name="minIdInfoRepository">Repository for MinId information operations.</param>
        public MinIdInfoManager(
            IMinIdInfoRepository minIdInfoRepository)
        {
            MinIdInfoRepository = minIdInfoRepository;
        }

        /// <summary>
        /// Creates a new MinId information record with validation to ensure business type uniqueness.
        /// This method enforces business rules to prevent duplicate business types within the system
        /// and logs the creation operation for audit purposes.
        /// </summary>
        /// <param name="minIdInfo">The MinId information entity to create. Cannot be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task containing the created MinId information entity.</returns>
        /// <exception cref="AbpException">Thrown when a MinId configuration with the same business type already exists.</exception>
        /// <exception cref="ArgumentNullException">Thrown when minIdInfo is null.</exception>
        public virtual async Task<MinIdInfo> CreateAsync(MinIdInfo minIdInfo, CancellationToken cancellationToken = default)
        {
            if (minIdInfo == null)
            {
                throw new ArgumentNullException(nameof(minIdInfo), "MinId information cannot be null when creating a new configuration.");
            }

            Logger.LogDebug("Attempting to create MinId configuration for business type '{BizType}'", minIdInfo.BizType);

            var queryMinIdInfo = await MinIdInfoRepository.FindExpectedByBizTypeAsync(minIdInfo.BizType, null, cancellationToken);
            if (queryMinIdInfo != null)
            {
                var errorMessage = $"Cannot create MinId configuration: Business type '{minIdInfo.BizType}' already exists in the system. Each business type must be unique.";
                Logger.LogWarning("Failed to create MinId configuration due to duplicate business type: {BizType}", minIdInfo.BizType);
                throw new AbpException(errorMessage);
            }

            var result = await MinIdInfoRepository.InsertAsync(minIdInfo, cancellationToken: cancellationToken);
            Logger.LogInformation("Successfully created MinId configuration for business type '{BizType}' with ID {Id}", minIdInfo.BizType, result.Id);
            
            return result;
        }

        /// <summary>
        /// Updates an existing MinId information record with new configuration values.
        /// This method validates that the updated business type doesn't conflict with existing records
        /// and applies the changes to the specified MinId configuration with comprehensive logging.
        /// </summary>
        /// <param name="id">The unique identifier of the MinId configuration to update.</param>
        /// <param name="bizType">The business type identifier for the configuration.</param>
        /// <param name="maxId">The maximum ID value for the configuration.</param>
        /// <param name="step">The step size for ID allocation.</param>
        /// <param name="delta">The delta value for modulo operations in distributed scenarios.</param>
        /// <param name="remainder">The remainder value for modulo operations.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task containing the updated MinId information entity.</returns>
        /// <exception cref="AbpException">Thrown when the specified ID is not found or when the business type conflicts with existing records.</exception>
        public virtual async Task<MinIdInfo> UpdateAsync(Guid id, string bizType, long maxId, int step, int delta, int remainder, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("Attempting to update MinId configuration with ID {Id} to business type '{BizType}'", id, bizType);

            var minIdInfo = await MinIdInfoRepository.GetAsync(id, cancellationToken: cancellationToken);
            if (minIdInfo == null)
            {
                var errorMessage = $"Cannot update MinId configuration: No configuration found with ID '{id}'. Please verify the ID is correct.";
                Logger.LogWarning("Failed to update MinId configuration: ID {Id} not found", id);
                throw new AbpException(errorMessage);
            }

            // Check for business type conflicts only if the business type is changing
            if (minIdInfo.BizType != bizType)
            {
                var queryMinIdInfo = await MinIdInfoRepository.FindExpectedByBizTypeAsync(bizType, id, cancellationToken);
                if (queryMinIdInfo != null)
                {
                    var errorMessage = $"Cannot update MinId configuration: Business type '{bizType}' already exists in another configuration (ID: {queryMinIdInfo.Id}). Each business type must be unique.";
                    Logger.LogWarning("Failed to update MinId configuration due to duplicate business type: {BizType} (existing ID: {ExistingId})", bizType, queryMinIdInfo.Id);
                    throw new AbpException(errorMessage);
                }
            }

            var oldBizType = minIdInfo.BizType;
            minIdInfo.Update(bizType, maxId, step, delta, remainder);

            var result = await MinIdInfoRepository.UpdateAsync(minIdInfo, cancellationToken: cancellationToken);
            Logger.LogInformation("Successfully updated MinId configuration {Id}: business type changed from '{OldBizType}' to '{NewBizType}', maxId: {MaxId}, step: {Step}, delta: {Delta}, remainder: {Remainder}", 
                id, oldBizType, bizType, maxId, step, delta, remainder);
            
            return result;
        }

    }
}
