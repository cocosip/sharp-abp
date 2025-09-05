using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DotCommon.Performance
{
    public class DefaultPerformanceServiceFactory : IPerformanceServiceFactory, ISingletonDependency
    {
        private readonly object _sync = new();
        private ConcurrentDictionary<string, IPerformanceService> _performanceServiceDict;
        protected ILogger Logger { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected IPerformanceConfigurationProvider ConfigurationProvider { get; }
        public DefaultPerformanceServiceFactory(
            ILogger<DefaultPerformanceServiceFactory> logger,
            IServiceProvider serviceProvider,
            IPerformanceConfigurationProvider configurationProvider)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            ConfigurationProvider = configurationProvider;
            _performanceServiceDict = new ConcurrentDictionary<string, IPerformanceService>();
        }

        /// <summary>
        /// GetOrCreate
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IPerformanceService GetOrCreate([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            if (!_performanceServiceDict.TryGetValue(name, out IPerformanceService performanceService))
            {
                lock (_sync)
                {
                    if (!_performanceServiceDict.TryGetValue(name, out performanceService))
                    {
                        var configuration = ConfigurationProvider.Get(name);
                        performanceService = ActivatorUtilities.CreateInstance<DefaultPerformanceService>(
                            ServiceProvider,
                            name,
                            configuration);

                        if (!_performanceServiceDict.TryAdd(name, performanceService))
                        {
                            Logger.LogInformation("Failed to add performance service '{PerformanceServiceName}' to dictionary.", name);
                        }
                        else
                        {
                            performanceService.Start();
                        }
                    }
                }
            }

            if (performanceService == null)
            {
                throw new AbpException($"Could not find performance service with the name '{name}'. Please check if the configuration exists and is properly formatted.");
            }

            return performanceService;
        }

        /// <summary>
        /// Stop all the performance
        /// </summary>
        public virtual void StopAll()
        {
            foreach (var performanceServiceKv in _performanceServiceDict)
            {
                performanceServiceKv.Value.Stop();
            }
        }
    }
}
