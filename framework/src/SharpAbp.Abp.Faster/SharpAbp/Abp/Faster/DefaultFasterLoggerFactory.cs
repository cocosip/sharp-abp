using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Faster
{
    /// <summary>
    /// Default implementation of <see cref="IFasterLoggerFactory"/> that creates and manages Faster logger instances
    /// </summary>
    public class DefaultFasterLoggerFactory : IFasterLoggerFactory, ISingletonDependency
    {
        /// <summary>
        /// Gets the concurrent dictionary that stores created logger instances with lazy initialization
        /// </summary>
        protected ConcurrentDictionary<string, Lazy<IFasterLogger>> Loggers { get; }
        
        /// <summary>
        /// Gets the ABP Faster options configuration
        /// </summary>
        protected AbpFasterOptions Options { get; }
        
        /// <summary>
        /// Gets the logger instance for internal logging
        /// </summary>
        protected ILogger Logger { get; }
        
        /// <summary>
        /// Gets the service provider for dependency resolution
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFasterLoggerFactory"/> class
        /// </summary>
        /// <param name="options">The ABP Faster options configuration</param>
        /// <param name="logger">The logger for internal logging</param>
        /// <param name="serviceProvider">The service provider for dependency resolution</param>
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

        /// <summary>
        /// Gets an existing logger or creates a new one with the specified name and type
        /// </summary>
        /// <typeparam name="T">The type associated with the logger</typeparam>
        /// <param name="name">The name of the logger to get or create</param>
        /// <returns>The Faster logger instance</returns>
        public virtual IFasterLogger<T> GetOrCreate<T>(string name) where T : class
        {
            // Use Lazy<T> to ensure thread-safe lazy initialization
            var lazyLogger = Loggers.GetOrAdd(name, key => new Lazy<IFasterLogger>(() => CreateLogger<T>(key)));
            Check.NotNull(lazyLogger.Value, nameof(IFasterLogger<T>));
            return (lazyLogger.Value as IFasterLogger<T>)!;
        }

        /// <summary>
        /// Creates a new Faster logger instance with the specified name and type
        /// </summary>
        /// <typeparam name="T">The type associated with the logger</typeparam>
        /// <param name="name">The name of the logger to create</param>
        /// <returns>A new Faster logger instance</returns>
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
