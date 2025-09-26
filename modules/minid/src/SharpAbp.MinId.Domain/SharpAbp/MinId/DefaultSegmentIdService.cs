﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Default implementation of segment ID service that manages ID segment allocation
    /// using database persistence with optimistic concurrency control and retry mechanisms.
    /// This service ensures thread-safe segment allocation across distributed instances.
    /// </summary>
    public class DefaultSegmentIdService : ISegmentIdService, ITransientDependency
    {
        /// <summary>
        /// Gets the logger instance for diagnostic and debugging purposes.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the MinId configuration options.
        /// </summary>
        protected MinIdOptions Options { get; }

        /// <summary>
        /// Gets the unit of work manager for transaction handling.
        /// </summary>
        protected IUnitOfWorkManager UnitOfWorkManager { get; }

        /// <summary>
        /// Gets the repository for MinId information persistence.
        /// </summary>
        protected IMinIdInfoRepository MinIdInfoRepository { get; }

        /// <summary>
        /// Initializes a new instance of the DefaultSegmentIdService.
        /// </summary>
        /// <param name="logger">Logger for diagnostic information.</param>
        /// <param name="options">Configuration options for MinId service.</param>
        /// <param name="unitOfWorkManager">Unit of work manager for transaction control.</param>
        /// <param name="minIdInfoRepository">Repository for MinId data operations.</param>
        public DefaultSegmentIdService(
            ILogger<DefaultSegmentIdService> logger,
            IOptions<MinIdOptions> options,
            IUnitOfWorkManager unitOfWorkManager,
            IMinIdInfoRepository minIdInfoRepository)
        {
            Logger = logger;
            Options = options.Value;
            UnitOfWorkManager = unitOfWorkManager;
            MinIdInfoRepository = minIdInfoRepository;
        }

        /// <summary>
        /// Retrieves the next available segment for the specified business type.
        /// This method implements optimistic concurrency control with retry logic
        /// to handle concurrent access in distributed environments.
        /// </summary>
        /// <param name="bizType">The business type identifier for segment allocation.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task containing the newly allocated segment with ID range information.</returns>
        /// <exception cref="AbpException">Thrown when the business type is not found or when all retry attempts fail due to conflicts.</exception>
        public virtual async Task<SegmentId> GetNextSegmentIdAsync(
            string bizType,
            CancellationToken cancellationToken = default)
        {
            var retry = Options.ConflictRetryCount;
            var timeout = Options.TransactionTimeout;

            for (var i = 0; i < retry; i++)
            {
                SegmentId segmentId = null;

                using var unitOfWork = UnitOfWorkManager.Begin(true, true, IsolationLevel.ReadCommitted, timeout);

                try
                {
                    var minIdInfo = await MinIdInfoRepository.FindByBizTypeAsync(bizType, cancellationToken)
                        ?? throw new AbpException($"Business type '{bizType}' is not registered in the MinId system. Please ensure the business type is properly configured.");

                    // Calculate new maximum ID by adding step to current maximum
                    var newMaxId = minIdInfo.MaxId + minIdInfo.Step;

                    Logger.LogDebug("Segment allocation for business type '{BizType}': Old maxId: {OldMaxId}, New maxId: {NewMaxId}, Step: {Step}",
                        bizType, minIdInfo.MaxId, newMaxId, minIdInfo.Step);

                    minIdInfo.UpdateMaxId(newMaxId);
                    await MinIdInfoRepository.UpdateAsync(minIdInfo, cancellationToken: cancellationToken);
                    segmentId = ConvertToSegmentId(minIdInfo);
                }
                catch
                {
                    try
                    {
                        await unitOfWork.RollbackAsync(cancellationToken);
                    }
                    catch
                    {
                        /* Transaction rollback failed - this is logged internally by ABP */
                    }

                    throw;
                }

                await unitOfWork.CompleteAsync(cancellationToken);

                if (segmentId != null)
                {
                    return segmentId;
                }
            }

            throw new AbpException($"Failed to allocate segment for business type '{bizType}' after {retry} retry attempts due to concurrent access conflicts. Please try again later.");
        }

        /// <summary>
        /// Converts MinIdInfo entity to SegmentId value object with calculated properties.
        /// This method calculates the segment boundaries, loading threshold, and other
        /// segment-specific properties based on the configuration options.
        /// </summary>
        /// <param name="minIdInfo">The MinId information entity containing segment data.</param>
        /// <returns>A SegmentId value object with calculated segment properties.</returns>
        protected virtual SegmentId ConvertToSegmentId(MinIdInfo minIdInfo)
        {
            var loadingPercent = Options.LoadingPercent;
            var segmentId = new SegmentId()
            {
                CurrentId = minIdInfo.MaxId - minIdInfo.Step,
                MaxId = minIdInfo.MaxId,
                Remainder = minIdInfo.Remainder < 0 ? 0 : minIdInfo.Remainder,
                Delta = minIdInfo.Delta,
                LoadingId = minIdInfo.MaxId - minIdInfo.Step + minIdInfo.Step * (loadingPercent / 100)
            };

            return segmentId;
        }

    }
}