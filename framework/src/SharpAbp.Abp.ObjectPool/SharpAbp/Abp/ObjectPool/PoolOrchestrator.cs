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
        /// Stores pool metadata for validation.
        /// </summary>
        protected class PoolMetadata
        {
            public Type PolicyType { get; set; }
            public int? MaxSize { get; set; }
            public object Pool { get; set; }

            public PoolMetadata(Type policyType, int? maxSize, object pool)
            {
                PolicyType = policyType;
                MaxSize = maxSize;
                Pool = pool;
            }
        }

        /// <summary>
        /// Gets the object pool provider.
        /// </summary>
        protected ObjectPoolProvider Provider { get; }

        /// <summary>
        /// Represents a composite key for pool identification.
        /// </summary>
        protected struct PoolKey : IEquatable<PoolKey>
        {
            public Type ObjectType { get; }
            public string PoolName { get; }

            public PoolKey(Type objectType, string poolName)
            {
                ObjectType = objectType;
                PoolName = poolName;
            }

            public bool Equals(PoolKey other)
            {
                return ObjectType == other.ObjectType && PoolName == other.PoolName;
            }

            public override bool Equals(object? obj)
            {
                return obj is PoolKey other && Equals(other);
            }

            public override int GetHashCode()
            {
#if NETSTANDARD2_0
                unchecked
                {
                    return ((ObjectType?.GetHashCode() ?? 0) * 397) ^ (PoolName?.GetHashCode() ?? 0);
                }
#else
                return HashCode.Combine(ObjectType, PoolName);
#endif
            }
        }

        /// <summary>
        /// Gets the dictionary of pool metadata, keyed by a composite key.
        /// </summary>
        protected ConcurrentDictionary<PoolKey, PoolMetadata> PoolMetadatas { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolOrchestrator"/> class.
        /// </summary>
        /// <param name="provider">The object pool provider.</param>
        public PoolOrchestrator(ObjectPoolProvider provider)
        {
            Provider = provider;
            PoolMetadatas = new ConcurrentDictionary<PoolKey, PoolMetadata>();
        }

        /// <summary>
        /// Gets or creates an object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policy">The policy to use for creating and resetting objects.</param>
        /// <param name="maxSize">The maximum size of the pool.</param>
        /// <returns>The object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        public virtual ObjectPool<T> GetPool<T>([NotNull] string poolName, IPooledObjectPolicy<T> policy, int? maxSize = null) where T : class
        {
            Check.NotNullOrWhiteSpace(poolName, nameof(poolName));
            Check.NotNull(policy, nameof(policy));

            var key = new PoolKey(typeof(T), poolName);
            var policyType = policy.GetType();

            var metadata = PoolMetadatas.GetOrAdd(key, _ =>
            {
                ObjectPool<T> pool;

                // If maxSize is specified, create a DefaultObjectPool with the specified size
                if (maxSize.HasValue)
                {
                    pool = new DefaultObjectPool<T>(policy, maxSize.Value);
                }
                else
                {
                    // Otherwise, use the injected Provider to create the pool
                    pool = Provider.Create(policy);
                }

                return new PoolMetadata(policyType, maxSize, pool);
            });

            // Validate that the parameters match the existing pool configuration
            if (metadata.PolicyType != policyType)
            {
                throw new InvalidOperationException(
                    $"Pool '{poolName}' for type '{typeof(T).FullName}' already exists with policy type '{metadata.PolicyType.FullName}', " +
                    $"but was requested with policy type '{policyType.FullName}'.");
            }

            if (metadata.MaxSize != maxSize)
            {
                throw new InvalidOperationException(
                    $"Pool '{poolName}' for type '{typeof(T).FullName}' already exists with maxSize '{metadata.MaxSize}', " +
                    $"but was requested with maxSize '{maxSize}'.");
            }

            return (ObjectPool<T>)metadata.Pool;
        }
    }
}