﻿using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Default implementation of MinId information querier with distributed caching support.
    /// This service provides efficient querying capabilities for MinId configurations
    /// by leveraging distributed caching to minimize database access and improve performance.
    /// </summary>
    public class MinIdInfoQuerier : IMinIdInfoQuerier, ITransientDependency
    {
        /// <summary>
        /// Gets the distributed cache for MinId information cache items.
        /// </summary>
        protected IDistributedCache<MinIdInfoCacheItem> MinIdInfoCache { get; }
        
        /// <summary>
        /// Gets the repository for MinId information persistence operations.
        /// </summary>
        protected IMinIdInfoRepository MinIdInfoRepository { get; }
        
        /// <summary>
        /// Gets the logger for diagnostic and performance monitoring purposes.
        /// </summary>
        protected ILogger<MinIdInfoQuerier> Logger { get; }

        /// <summary>
        /// Initializes a new instance of the MinIdInfoQuerier with required dependencies.
        /// </summary>
        /// <param name="minIdInfoCache">Distributed cache for MinId information.</param>
        /// <param name="minIdInfoRepository">Repository for MinId data operations.</param>
        /// <param name="logger">Logger for diagnostic information.</param>
        public MinIdInfoQuerier(
            IDistributedCache<MinIdInfoCacheItem> minIdInfoCache,
            IMinIdInfoRepository minIdInfoRepository,
            ILogger<MinIdInfoQuerier> logger)
        {
            MinIdInfoCache = minIdInfoCache;
            MinIdInfoRepository = minIdInfoRepository;
            Logger = logger;
        }

        /// <summary>
        /// Checks whether a MinId configuration exists for the specified business type.
        /// This method first attempts to retrieve the information from the distributed cache.
        /// If not found in cache, it queries the database and caches the result for future requests.
        /// </summary>
        /// <param name="bizType">The business type identifier to check for existence. Cannot be null or whitespace.</param>
        /// <returns>A task containing true if a MinId configuration exists for the business type; otherwise, false.</returns>
        /// <exception cref="System.ArgumentException">Thrown when bizType is null or whitespace.</exception>
        public virtual async Task<bool> ExistAsync([NotNull] string bizType)
        {
            Check.NotNullOrWhiteSpace(bizType, nameof(bizType));

            Logger.LogDebug("Checking existence of MinId configuration for business type '{BizType}'", bizType);

            var minIdInfoCacheItem = await MinIdInfoCache.GetOrAddAsync(bizType, async () =>
            {
                Logger.LogDebug("Cache miss for business type '{BizType}', querying database", bizType);
                var minIdInfo = await MinIdInfoRepository.FindByBizTypeAsync(bizType);
                
                if (minIdInfo != null)
                {
                    Logger.LogDebug("Found MinId configuration for business type '{BizType}', caching result", bizType);
                    return minIdInfo.ToCacheItem();
                }
                
                Logger.LogDebug("No MinId configuration found for business type '{BizType}'", bizType);
                return null;
            });

            var exists = minIdInfoCacheItem != null;
            Logger.LogDebug("MinId configuration existence check for business type '{BizType}': {Exists}", bizType, exists);
            
            return exists;
        }
    }
}
