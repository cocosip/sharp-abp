using JetBrains.Annotations;
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
    public class DefaultMinIdGeneratorFactory : IMinIdGeneratorFactory, ISingletonDependency
    {
        private readonly int _getMinIdGeneratorTimeoutMillis;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly ConcurrentDictionary<string, IMinIdGenerator> _minIdGenerators;

        protected ILogger Logger { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected IMinIdInfoQuerier Querier { get; }
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
        /// Get minIdGenerator
        /// </summary>
        /// <param name="bizType"></param>
        /// <returns></returns>
        public virtual async Task<IMinIdGenerator> GetAsync(string bizType)
        {
            if (!_minIdGenerators.TryGetValue(bizType, out IMinIdGenerator minIdGenerator))
            {
                if (!await _semaphoreSlim.WaitAsync(_getMinIdGeneratorTimeoutMillis))
                {
                    throw new AbpException("Get minIdGenerator timeout.");
                }

                try
                {
                    if (!_minIdGenerators.TryGetValue(bizType, out minIdGenerator))
                    {
                        minIdGenerator = await CreateAsync(bizType);
                        if (!_minIdGenerators.TryAdd(bizType, minIdGenerator))
                        {
                            Logger.LogWarning("Failed to add minIdGenerator to dict.");
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
        /// Create minIdGenerator by bizType
        /// </summary>
        /// <param name="bizType"></param>
        /// <returns></returns>
        protected virtual async Task<IMinIdGenerator> CreateAsync([NotNull] string bizType)
        {
            Check.NotNullOrWhiteSpace(bizType, nameof(bizType));
            if (!await Querier.ExistAsync(bizType))
            {
                throw new AbpException($"Could not find bizType '{bizType}'.");
            }

            var minIdGenerator = ActivatorUtilities.CreateInstance<DefaultMinIdGenerator>(ServiceProvider, bizType);
            return minIdGenerator;
        }



    }
}
