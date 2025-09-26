﻿﻿﻿﻿﻿﻿using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Default implementation of minimum ID generator using segment-based allocation strategy.
    /// This generator manages two segments (current and next) to ensure continuous ID generation
    /// with automatic segment switching and pre-loading capabilities.
    /// </summary>
    public class DefaultMinIdGenerator : IMinIdGenerator, ITransientDependency
    {
        private volatile SegmentId _current = null;
        private volatile SegmentId _next = null;
        private readonly int _loadSegmentIdTimeoutMillis = 5000;
        private bool _isLoadingNext = false;
        private readonly SemaphoreSlim _semaphoreSlim;
        
        /// <summary>
        /// Gets the segment ID service for managing segment allocation.
        /// </summary>
        protected ISegmentIdService SegmentIdService { get; }

        /// <summary>
        /// Gets the business type identifier for this generator instance.
        /// </summary>
        public string BizType { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the DefaultMinIdGenerator.
        /// </summary>
        /// <param name="bizType">The business type identifier for segment allocation.</param>
        /// <param name="options">Configuration options for the MinId generator.</param>
        /// <param name="segmentIdService">Service for managing segment operations.</param>
        public DefaultMinIdGenerator(string bizType, IOptions<MinIdOptions> options, ISegmentIdService segmentIdService)
        {
            BizType = bizType;
            _loadSegmentIdTimeoutMillis = options.Value.LoadSegmentIdTimeoutMillis;

            SegmentIdService = segmentIdService;
            _semaphoreSlim = new SemaphoreSlim(1);
        }

        /// <summary>
        /// Loads the current segment for ID generation.
        /// If the current segment is null or exhausted, this method will either promote 
        /// the next segment to current or acquire a new segment from the service.
        /// </summary>
        /// <returns>A task representing the asynchronous load operation.</returns>
        /// <exception cref="AbpException">Thrown when segment loading times out.</exception>
        public virtual async Task LoadCurrentAsync()
        {
            if (_current == null || !_current.IsUseful())
            {
                if (!await _semaphoreSlim.WaitAsync(_loadSegmentIdTimeoutMillis))
                {
                    throw new AbpException($"Failed to load current segment within the timeout period of {_loadSegmentIdTimeoutMillis}ms for business type '{BizType}'.");
                }

                try
                {
                    if (_next == null)
                    {
                        var segmentId = await QuerySegmentIdAsync();
                        _current = segmentId;
                    }
                    else
                    {
                        _current = _next;
                        _next = null;
                    }
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }
        }

        /// <summary>
        /// Pre-loads the next segment for ID generation to ensure continuous allocation.
        /// This method is called when the current segment is nearing exhaustion to prevent
        /// service interruption during segment transitions.
        /// </summary>
        /// <returns>A task representing the asynchronous pre-load operation.</returns>
        /// <exception cref="AbpException">Thrown when segment pre-loading times out.</exception>
        public virtual async Task LoadNextAsync()
        {
            if (_next == null && !_isLoadingNext)
            {
                if (!await _semaphoreSlim.WaitAsync(_loadSegmentIdTimeoutMillis))
                {
                    throw new AbpException($"Failed to pre-load next segment within the timeout period of {_loadSegmentIdTimeoutMillis}ms for business type '{BizType}'.");
                }

                try
                {
                    if (_next == null && !_isLoadingNext)
                    {
                        _isLoadingNext = true;
                        try
                        {
                            _next = await QuerySegmentIdAsync();
                        }
                        finally
                        {
                            _isLoadingNext = false;
                        }
                    }
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }
        }

        /// <summary>
        /// Generates the next unique identifier from the current segment.
        /// This method automatically handles segment loading and transitions to ensure
        /// continuous ID generation without interruption.
        /// </summary>
        /// <returns>A task containing the next unique identifier.</returns>
        public virtual async Task<long> NextIdAsync()
        {
            while (true)
            {
                if (_current == null)
                {
                    await LoadCurrentAsync();
                    continue;
                }

                var (code, id) = _current.NextId();

                if (code == ResultCodeConsts.Over)
                {
                    await LoadCurrentAsync();
                }
                else
                {
                    if (code == ResultCodeConsts.Loading)
                    {
                        await LoadNextAsync();
                    }
                    return id;
                }
            }
        }

        /// <summary>
        /// Generates a batch of unique identifiers with the specified size.
        /// This method efficiently generates multiple IDs by calling NextIdAsync sequentially
        /// for the specified number of times.
        /// </summary>
        /// <param name="batchSize">The number of unique identifiers to generate. Must be positive.</param>
        /// <returns>A task containing a list of unique identifiers.</returns>
        public virtual async Task<List<long>> NextIdAsync(int batchSize)
        {
            var ids = new List<long>();
            for (var i = 0; i < batchSize; i++)
            {
                var id = await NextIdAsync();
                ids.Add(id);
            }
            return ids;
        }

        /// <summary>
        /// Queries and retrieves a new segment from the segment service.
        /// This method encapsulates the interaction with the segment service and
        /// provides enhanced error handling with context information.
        /// </summary>
        /// <returns>A task containing the newly acquired segment.</returns>
        /// <exception cref="AbpException">Thrown when segment acquisition fails with detailed error information.</exception>
        protected virtual async Task<SegmentId> QuerySegmentIdAsync()
        {
            try
            {
                var segmentId = await SegmentIdService.GetNextSegmentIdAsync(BizType);
                return segmentId;
            }
            catch (Exception ex)
            {
                throw new AbpException($"Failed to acquire new segment for business type '{BizType}'. Error details: {ex.Message}", ex);
            }
        }

    }
}
