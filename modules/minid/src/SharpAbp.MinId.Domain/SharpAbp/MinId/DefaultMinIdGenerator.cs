using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.MinId
{
    public class DefaultMinIdGenerator : IMinIdGenerator, ITransientDependency
    {

        private volatile SegmentId _current = null;
        private volatile SegmentId _next = null;
        private readonly int _loadSegmentIdTimeoutMillis = 5000;
        private bool _isLoadingNext = false;
        private readonly SemaphoreSlim _semaphoreSlim;
        protected ISegmentIdService SegmentIdService { get; }

        public string BizType { get; protected set; }

        public DefaultMinIdGenerator(string bizType, IOptions<MinIdOptions> options, ISegmentIdService segmentIdService)
        {
            BizType = bizType;
            _loadSegmentIdTimeoutMillis = options.Value.LoadSegmentIdTimeoutMillis;

            SegmentIdService = segmentIdService;
            _semaphoreSlim = new SemaphoreSlim(1);
        }

        /// <summary>
        /// Load current segmentId
        /// </summary>
        /// <returns></returns>
        public virtual async Task LoadCurrentAsync()
        {
            if (_current == null || !_current.IsUseful())
            {
                if (!await _semaphoreSlim.WaitAsync(_loadSegmentIdTimeoutMillis))
                {
                    throw new AbpException("Load current segmentId time out.");
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
        /// Load next segmentId
        /// </summary>
        /// <returns></returns>
        public virtual async Task LoadNextAsync()
        {
            if (_next == null && !_isLoadingNext)
            {
                if (!await _semaphoreSlim.WaitAsync(_loadSegmentIdTimeoutMillis))
                {
                    throw new AbpException("Load next segmentId time out.");
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
        /// NextId
        /// </summary>
        /// <returns></returns>
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
        /// Batch nextId
        /// </summary>
        /// <param name="batchSize"></param>
        /// <returns></returns>
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
        /// Query segmentId
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<SegmentId> QuerySegmentIdAsync()
        {
            try
            {
                var segmentId = await SegmentIdService.GetNextSegmentIdAsync(BizType);
                return segmentId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Query segmentId error:{ex.Message}");
            }
        }

    }
}
