using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;
using Microsoft.Extensions.ObjectPool;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.ObjectPool
{
    /// <summary>
    /// Orchestrates object pools for different types and names.
    /// </summary>
    public class PoolOrchestrator : IPoolOrchestrator, ISingletonDependency
    {
        /// <summary>
        /// Gets the object pool provider.
        /// </summary>
        protected ObjectPoolProvider Provider { get; }

        /// <summary>
        /// Gets the dictionary of pools, keyed by a normalized string.
        /// </summary>
        protected ConcurrentDictionary<string, object> Pools { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolOrchestrator"/> class.
        /// </summary>
        /// <param name="provider">The object pool provider.</param>
        public PoolOrchestrator(ObjectPoolProvider provider)
        {
            Provider = provider;
            Pools = new ConcurrentDictionary<string, object>();
        }

        /// <summary>
        /// Gets or creates an object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policy">The policy to use for creating and resetting objects.</param>
        /// <param name="maxSize">The maximum size of the pool.</param>
        /// <returns>The object pool.</returns>
        public virtual ObjectPool<T> GetPool<T>([NotNull] string poolName, IPooledObjectPolicy<T> policy, int? maxSize = null) where T : class
        {
            Check.NotNullOrWhiteSpace(poolName, nameof(poolName));
            Check.NotNull(policy, nameof(policy));
            var key = NormalizeKey(typeof(T), poolName);
            return (ObjectPool<T>)Pools.GetOrAdd(key, _ =>
            {
                var configurableObjectPoolProvider = new ConfigurableObjectPoolProvider
                {
                    MaxSize = maxSize ?? Provider.GetDefaultSize()
                };
                return configurableObjectPoolProvider.Create(policy);
            });
        }

        /// <summary>
        /// Normalizes the key for the pool dictionary.
        /// </summary>
        /// <param name="t">The type of objects in the pool.</param>
        /// <param name="poolName">The name of the pool.</param>
        /// <returns>The normalized key.</returns>
        protected virtual string NormalizeKey(Type t, string poolName)
        {
            return $"{t.FullName}-{poolName}";
        }
    }
}