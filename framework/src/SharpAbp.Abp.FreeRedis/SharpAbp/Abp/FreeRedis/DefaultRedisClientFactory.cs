using FreeRedis;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FreeRedis
{
    public class DefaultRedisClientFactory : IRedisClientFactory, ISingletonDependency, IDisposable
    {
        private readonly ConcurrentDictionary<string, Lazy<RedisClient>> _clientDict;
        private readonly object _lifecycle = new object();
        private readonly HashSet<Lazy<RedisClient>> _initializing = new HashSet<Lazy<RedisClient>>();
        private readonly AsyncLocal<InitializationScope?> _initialization = new AsyncLocal<InitializationScope?>();
        private bool _disposed;

        private sealed class InitializationScope(InitializationScope? parent)
        {
            public InitializationScope? Parent { get; } = parent;
            public int Active = 1;
        }

        protected ILogger Logger { get; }
        protected IRedisConfigurationProvider ConfigurationSelector { get; }
        protected IRedisClientBuilder ClientBuilder { get; }
        public DefaultRedisClientFactory(
            ILogger<DefaultRedisClientFactory> logger,
            IRedisConfigurationProvider configurationSelector,
            IRedisClientBuilder clientBuilder)
        {
            Logger = logger;
            ConfigurationSelector = configurationSelector;
            ClientBuilder = clientBuilder;

            _clientDict = new ConcurrentDictionary<string, Lazy<RedisClient>>();
        }

        /// <summary>
        /// Gets a shared client owned by this factory. Callers must not dispose it.
        /// </summary>
        /// <param name="name">name</param>
        /// <returns></returns>
        [NotNull]
        public virtual RedisClient Get([NotNull] string name = DefaultClient.Name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            Lazy<RedisClient> lazy;
            lock (_lifecycle)
            {
                ThrowIfDisposed();
                lazy = _clientDict.GetOrAdd(name, n => new Lazy<RedisClient>(() =>
                {
                    var configuration = ConfigurationSelector.Get(n);
                    if (configuration == null)
                    {
                        throw new AbpException($"Could not find configuration by name '{n}'");
                    }
                    return ClientBuilder.CreateClient(configuration);
                }));
                _initializing.Add(lazy);
            }
            var previous = _initialization.Value;
            var scope = new InitializationScope(previous);
            _initialization.Value = scope;
            try
            {
                return lazy.Value;
            }
            catch
            {
                ((ICollection<KeyValuePair<string, Lazy<RedisClient>>>)_clientDict).Remove(new(name, lazy));
                throw;
            }
            finally
            {
                Volatile.Write(ref scope.Active, 0);
                _initialization.Value = previous;
                lock (_lifecycle) _initializing.Remove(lazy);
            }
        }

        /// <summary>
        /// Gets the initialized shared clients. Callers must not dispose them.
        /// </summary>
        /// <returns></returns>
        public List<RedisClient> GetAll()
        {
            lock (_lifecycle)
            {
                ThrowIfDisposed();
                return _clientDict.Values.Where(client => client.IsValueCreated).Select(client => client.Value).ToList();
            }
        }

        public void Dispose()
        {
            List<Lazy<RedisClient>> clients;
            lock (_lifecycle)
            {
                if (_disposed) return;
                RejectInitializationDisposal();
                _disposed = true;
                clients = _clientDict.Values.Where(lazy => lazy.IsValueCreated || _initializing.Contains(lazy)).ToList();
                _clientDict.Clear();
            }

            var errors = new List<Exception>();
            foreach (var lazy in clients)
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
    }
}
