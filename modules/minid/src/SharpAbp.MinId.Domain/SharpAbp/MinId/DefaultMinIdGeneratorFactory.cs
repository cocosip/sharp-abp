﻿using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Default implementation of the MinId generator factory.
    /// Manages creation and caching of MinId generators for different business types.
    /// </summary>
    public class DefaultMinIdGeneratorFactory : IMinIdGeneratorFactory, ISingletonDependency
    {
        private readonly int _getMinIdGeneratorTimeoutMillis;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly ConcurrentDictionary<string, IMinIdGenerator> _minIdGenerators;

        protected ILogger Logger { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected IMinIdInfoQuerier Querier { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMinIdGeneratorFactory"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="serviceProvider">The service provider for dependency injection.</param>
        /// <param name="options">The MinId options containing configuration values.</param>
        /// <param name="querier">The MinId info querier for checking business type existence.</param>
        public DefaultMinIdGeneratorFactory(
            ILogger<DefaultMinIdGeneratorFactory> logger,
            IServiceProvider serviceProvider,
            IOptions<MinIdOptions> options,
            IMinIdInfoQuerier querier)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            _getMinIdGeneratorTimeoutMillis = options.Value.GetMinIdGeneratorTimeoutMillis;
            Querier = querier;
            _semaphoreSlim = new SemaphoreSlim(1);
            _minIdGenerators = new ConcurrentDictionary<string, IMinIdGenerator>();
        }

        /// <summary>
        /// Gets a MinId generator instance for the specified business type.
        /// If a generator for the business type already exists in the cache, it is returned.
        /// Otherwise, a new generator is created and added to the cache.
        /// </summary>
        /// <param name="bizType">The business type identifier for which to retrieve the generator.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the MinId generator instance.</returns>
        /// <exception cref="AbpException">Thrown when the operation times out or when the business type is not found.</exception>
        public virtual async Task<IMinIdGenerator> GetAsync(string bizType)
        {
            if (!_minIdGenerators.TryGetValue(bizType, out IMinIdGenerator minIdGenerator))
            {
                if (!await _semaphoreSlim.WaitAsync(_getMinIdGeneratorTimeoutMillis))
                {
                    throw new AbpException($"Failed to acquire lock for MinId generator within {_getMinIdGeneratorTimeoutMillis} milliseconds. This may indicate high contention or a performance issue.");
                }

                try
                {
                    if (!_minIdGenerators.TryGetValue(bizType, out minIdGenerator))
                    {
                        minIdGenerator = await CreateAsync(bizType);
                        if (!_minIdGenerators.TryAdd(bizType, minIdGenerator))
                        {
                            Logger.LogWarning("Failed to cache MinId generator for business type '{BizType}'. This may cause performance degradation due to repeated creation.", bizType);
                        }
                    }
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }

            return minIdGenerator;
        }

        /// <summary>
        /// Creates a new MinId generator instance for the specified business type.
        /// </summary>
        /// <param name="bizType">The business type for which to create the generator.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the newly created MinId generator instance.</returns>
        /// <exception cref="AbpException">Thrown when the specified business type does not exist.</exception>
        protected virtual async Task<IMinIdGenerator> CreateAsync([NotNull] string bizType)
        {
            Check.NotNullOrWhiteSpace(bizType, nameof(bizType));
            if (!await Querier.ExistAsync(bizType))
            {
                throw new AbpException($"No MinId configuration found for business type '{bizType}'. Please ensure the business type is properly configured in the MinId system.");
            }

            var minIdGenerator = ActivatorUtilities.CreateInstance<DefaultMinIdGenerator>(ServiceProvider, bizType);
            return minIdGenerator;
        }
    }
}