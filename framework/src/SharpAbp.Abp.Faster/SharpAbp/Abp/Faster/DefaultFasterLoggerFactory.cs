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

        protected ConcurrentDictionary<string, Lazy<IFasterLogger>> Loggers { get; }
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
            Loggers = [];
        }


        public virtual IFasterLogger<T> GetOrCreate<T>(string name) where T : class
        {
            // 使用 Lazy<T> 确保线程安全的延迟初始化
            var lazyLogger = Loggers.GetOrAdd(name, key => new Lazy<IFasterLogger>(() => CreateLogger<T>(key)));
            return lazyLogger.Value as IFasterLogger<T>;
        }

        protected virtual IFasterLogger<T> CreateLogger<T>(string name) where T : class
        {
            var configuration = Options.Configurations.GetConfiguration(name);
            var fasterLogger = ActivatorUtilities.CreateInstance<FasterLogger<T>>(
                ServiceProvider,
                name,
                configuration);
            fasterLogger.Initialize();
            return fasterLogger;
        }

    }
}
