using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Faster
{
    /// <summary>
    /// Default implementation of <see cref="IFasterLoggerFactory"/> that creates and manages Faster logger instances
    /// </summary>
    public class DefaultFasterLoggerFactory : IFasterLoggerFactory, ISingletonDependency, IDisposable
    {
        private readonly object _lifecycle = new object();
        private readonly HashSet<Lazy<IFasterLogger>> _initializing = new HashSet<Lazy<IFasterLogger>>();
        private readonly AsyncLocal<InitializationScope?> _initialization = new AsyncLocal<InitializationScope?>();
        private bool _disposed;

        private sealed class InitializationScope(InitializationScope? parent)
        {
            public InitializationScope? Parent { get; } = parent;
            public int Active = 1;
        }
        /// <summary>
        /// Gets the concurrent dictionary that stores created logger instances with lazy initialization
        /// </summary>
        protected ConcurrentDictionary<LoggerCacheKey, Lazy<IFasterLogger>> Loggers { get; }
        
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
            var cacheKey = new LoggerCacheKey(typeof(T), name);
            Lazy<IFasterLogger> lazyLogger;
            lock (_lifecycle)
            {
                ThrowIfDisposed();
                lazyLogger = Loggers.GetOrAdd(cacheKey, key => new Lazy<IFasterLogger>(() => CreateLogger<T>(key.Name)));
                _initializing.Add(lazyLogger);
            }
            var previous = _initialization.Value;
            var scope = new InitializationScope(previous);
            _initialization.Value = scope;
            try
            {
                Check.NotNull(lazyLogger.Value, nameof(IFasterLogger<T>));
                return (lazyLogger.Value as IFasterLogger<T>)!;
            }
            catch
            {
                // Remove only the failed entry so a concurrent replacement can survive.
                ((ICollection<KeyValuePair<LoggerCacheKey, Lazy<IFasterLogger>>>)Loggers).Remove(new(cacheKey, lazyLogger));
                throw;
            }
            finally
            {
                Volatile.Write(ref scope.Active, 0);
                _initialization.Value = previous;
                lock (_lifecycle) _initializing.Remove(lazyLogger);
            }
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
            try
            {
                fasterLogger.Initialize();
                return fasterLogger;
            }
            catch (Exception initializationError)
            {
                try
                {
                    fasterLogger.Dispose();
                }
                catch (Exception cleanupError)
                {
                    throw new AggregateException(initializationError, cleanupError);
                }
                throw;
            }
        }

        /// <summary>
        /// Disposes all created logger instances and releases resources
        /// </summary>
        public void Dispose()
        {
            List<Lazy<IFasterLogger>> loggers;
            lock (_lifecycle)
            {
                if (_disposed) return;
                RejectInitializationDisposal();
                _disposed = true;
                loggers = Loggers.Values.Where(lazy => lazy.IsValueCreated || _initializing.Contains(lazy)).ToList();
                Loggers.Clear();
            }

            var errors = new List<Exception>();
            foreach (var lazy in loggers)
            {
                try
                {
                    lazy.Value?.Dispose();
                }
                catch (Exception error)
                {
                    errors.Add(error);
                }
            }

            if (errors.Count > 0) throw new AggregateException(errors);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().Name);
        }

        private void RejectInitializationDisposal()
        {
            for (var scope = _initialization.Value; scope != null; scope = scope.Parent)
            {
                if (Volatile.Read(ref scope.Active) != 0)
                    throw new InvalidOperationException("Factory disposal cannot run inside this factory's initialization callback.");
            }
        }

        protected readonly struct LoggerCacheKey : IEquatable<LoggerCacheKey>
        {
            public Type Type { get; }

            public string Name { get; }

            public LoggerCacheKey(Type type, string name)
            {
                Type = type;
                Name = name;
            }

            public bool Equals(LoggerCacheKey other)
            {
                return Type == other.Type && string.Equals(Name, other.Name, StringComparison.Ordinal);
            }

            public override bool Equals(object? obj)
            {
                return obj is LoggerCacheKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Type?.GetHashCode() ?? 0) * 397) ^ StringComparer.Ordinal.GetHashCode(Name);
                }
            }
        }
    }
}
