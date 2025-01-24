using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Faster
{
    public class DefaultFasterLoggerFactory : IFasterLoggerFactory, ISingletonDependency
    {
        private readonly object _lock = new();
        protected ConcurrentDictionary<string, IFasterLogger> _loggers;
        protected AbpFasterOptions Options { get; }
        protected ILogger Logger { get; }
        protected IServiceProvider ServiceProvider { get; }

        public DefaultFasterLoggerFactory(
            IOptions<AbpFasterOptions> options,
            ILogger<DefaultFasterLoggerFactory> logger,
            IServiceProvider serviceProvider)
        {
            Options = options.Value;
            Logger = logger;
            ServiceProvider = serviceProvider;
            _loggers = new ConcurrentDictionary<string, IFasterLogger>();
        }


        public virtual IFasterLogger<T> GetOrCreate<T>(string name) where T : class
        {
            lock (_lock)
            {
                var configuration = Options.Configurations.GetConfiguration(name);
                if (_loggers.TryGetValue(name, out IFasterLogger logger))
                {
                    return logger as IFasterLogger<T>;
                }

                var fasterLogger = ActivatorUtilities.CreateInstance<FasterLogger<T>>(
                      ServiceProvider,
                      name,
                      configuration);
                fasterLogger.Initialize();
                if (!_loggers.TryAdd(name, fasterLogger))
                {
                    Logger.LogDebug("GetOrCreate faster logger add failed. {name}", name);
                }
                return fasterLogger;
            }
        }

    }
}
