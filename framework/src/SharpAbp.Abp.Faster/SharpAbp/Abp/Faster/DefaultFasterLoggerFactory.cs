using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Faster
{
    public class DefaultFasterLoggerFactory : IFasterLoggerFactory, ISingletonDependency
    {
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
            _loggers = [];
        }


        public virtual IFasterLogger<T> GetOrCreate<T>(string name) where T : class
        {
            var configuration = Options.Configurations.GetConfiguration(name);
            var logger = _loggers.GetOrAdd(name, () =>
            {
                var fasterLogger = ActivatorUtilities.CreateInstance<FasterLogger<T>>(
                   ServiceProvider,
                   name,
                   configuration);
                fasterLogger.Initialize();
                return fasterLogger;
            }).As<IFasterLogger<T>>();

            return logger;
        }

    }
}
