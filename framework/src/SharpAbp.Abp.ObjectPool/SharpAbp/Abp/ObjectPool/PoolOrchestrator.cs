using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.Extensions.ObjectPool;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.ObjectPool
{
    /// <summary>
    /// Orchestrates object pools for different types and names.
    /// </summary>
    public class PoolOrchestrator : IPoolOrchestrator, ISingletonDependency, IDisposable
    {
        private readonly object _lifecycle = new object();
        private readonly HashSet<Lazy<PoolMetadata>> _initializing = new HashSet<Lazy<PoolMetadata>>();
        private readonly AsyncLocal<InitializationScope?> _initialization = new AsyncLocal<InitializationScope?>();
        private bool _disposed;

        private sealed class InitializationScope(InitializationScope? parent)
        {
            public InitializationScope? Parent { get; } = parent;
            public int Active = 1;
        }
        /// <summary>
        /// Stores pool metadata for validation.
        /// </summary>
        protected class PoolMetadata
        {
            /// <summary>
            /// The concrete policy type when it is known; null when the pool was created
            /// through a factory overload and the type cannot be inspected.
            /// </summary>
            public Type? PolicyType { get; set; }

            public int MaxSize { get; set; }

            public object Pool { get; set; }

            public PoolMetadata(Type? policyType, int maxSize, object pool)
            {
                PolicyType = policyType;
                MaxSize = maxSize;
                Pool = pool;
            }
        }

        /// <summary>
        /// Represents a composite key for pool identification.
        /// </summary>
        protected struct PoolKey : IEquatable<PoolKey>
        {
            public Type ObjectType { get; }
            public string PoolName { get; }
            public bool IsAsync { get; }

            public PoolKey(Type objectType, string poolName, bool isAsync = false)
            {
                ObjectType = objectType;
                PoolName = poolName;
                IsAsync = isAsync;
            }

            public bool Equals(PoolKey other)
            {
                return ObjectType == other.ObjectType && PoolName == other.PoolName && IsAsync == other.IsAsync;
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
                    return (((ObjectType?.GetHashCode() ?? 0) * 397) ^ (PoolName?.GetHashCode() ?? 0)) * 397 ^ IsAsync.GetHashCode();
                }
#else
                return HashCode.Combine(ObjectType, PoolName, IsAsync);
#endif
            }
        }

        /// <summary>
        /// Gets the dictionary of pool metadata, keyed by a composite key.
        /// </summary>
        protected ConcurrentDictionary<PoolKey, Lazy<PoolMetadata>> PoolMetadatas { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolOrchestrator"/> class.
        /// </summary>
        public PoolOrchestrator()
        {
            PoolMetadatas = new ConcurrentDictionary<PoolKey, Lazy<PoolMetadata>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolOrchestrator"/> class.
        /// </summary>
        /// <param name="provider">Kept for backward compatibility; pools are created by this orchestrator.</param>
        public PoolOrchestrator(ObjectPoolProvider provider) : this()
        {
        }

        /// <summary>
        /// Gets or creates an object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policy">The policy to use for creating and resetting objects.</param>
        /// <param name="maxSize">The maximum size of the pool. Defaults to <see cref="ObjectPoolDefaults.GetDefaultMaximumRetained"/>.</param>
        /// <returns>The object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        public virtual ObjectPool<T> GetPool<T>([NotNull] string poolName, IPooledObjectPolicy<T> policy, int? maxSize = null) where T : class
        {
            Check.NotNullOrWhiteSpace(poolName, nameof(poolName));
            Check.NotNull(policy, nameof(policy));

            return GetPool(poolName, () => policy, policy.GetType(), maxSize);
        }

        /// <summary>
        /// Gets or creates an object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policyFactory">The factory to create the policy only when the pool is first created.</param>
        /// <param name="maxSize">The maximum size of the pool. Defaults to <see cref="ObjectPoolDefaults.GetDefaultMaximumRetained"/>.</param>
        /// <returns>The object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        public virtual ObjectPool<T> GetPool<T>([NotNull] string poolName, Func<IPooledObjectPolicy<T>> policyFactory, int? maxSize = null) where T : class
        {
            Check.NotNullOrWhiteSpace(poolName, nameof(poolName));
            Check.NotNull(policyFactory, nameof(policyFactory));

            return GetPool(poolName, policyFactory, null, maxSize);
        }

        /// <summary>
        /// Gets or creates an object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <typeparam name="TPolicy">The type of policy used by the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policyFactory">The factory to create the policy only when the pool is first created.</param>
        /// <param name="maxSize">The maximum size of the pool. Defaults to <see cref="ObjectPoolDefaults.GetDefaultMaximumRetained"/>.</param>
        /// <returns>The object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        public virtual ObjectPool<T> GetPool<T, TPolicy>([NotNull] string poolName, Func<TPolicy> policyFactory, int? maxSize = null)
            where T : class
            where TPolicy : IPooledObjectPolicy<T>
        {
            Check.NotNullOrWhiteSpace(poolName, nameof(poolName));
            Check.NotNull(policyFactory, nameof(policyFactory));

            return GetPool(poolName, () => policyFactory(), typeof(TPolicy), maxSize);
        }

        /// <summary>
        /// Gets or creates an object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policy">The policy to use for creating and resetting objects.</param>
        /// <param name="maxSize">The maximum size of the pool. Defaults to <see cref="ObjectPoolDefaults.GetDefaultMaximumRetained"/>.</param>
        /// <returns>The object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        public virtual IObjectPool<T> GetObjectPool<T>([NotNull] string poolName, IObjectPoolPolicy<T> policy, int? maxSize = null) where T : class
        {
            Check.NotNullOrWhiteSpace(poolName, nameof(poolName));
            Check.NotNull(policy, nameof(policy));

            var pool = GetPool<T>(poolName, () => new ObjectPoolPolicyAdapter<T>(policy), policy.GetType(), maxSize);
            return new ObjectPoolAdapter<T>(pool);
        }

        /// <summary>
        /// Gets or creates an object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policyFactory">The factory to create the policy only when the pool is first created.</param>
        /// <param name="maxSize">The maximum size of the pool. Defaults to <see cref="ObjectPoolDefaults.GetDefaultMaximumRetained"/>.</param>
        /// <returns>The object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        public virtual IObjectPool<T> GetObjectPool<T>([NotNull] string poolName, Func<IObjectPoolPolicy<T>> policyFactory, int? maxSize = null) where T : class
        {
            Check.NotNullOrWhiteSpace(poolName, nameof(poolName));
            Check.NotNull(policyFactory, nameof(policyFactory));

            return GetObjectPool<T>(poolName, policyFactory, null, maxSize);
        }

        /// <summary>
        /// Gets or creates an object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <typeparam name="TPolicy">The type of policy used by the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policyFactory">The factory to create the policy only when the pool is first created.</param>
        /// <param name="maxSize">The maximum size of the pool. Defaults to <see cref="ObjectPoolDefaults.GetDefaultMaximumRetained"/>.</param>
        /// <returns>The object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        public virtual IObjectPool<T> GetObjectPool<T, TPolicy>([NotNull] string poolName, Func<TPolicy> policyFactory, int? maxSize = null)
            where T : class
            where TPolicy : IObjectPoolPolicy<T>
        {
            Check.NotNullOrWhiteSpace(poolName, nameof(poolName));
            Check.NotNull(policyFactory, nameof(policyFactory));

            return GetObjectPool<T>(poolName, () => policyFactory(), typeof(TPolicy), maxSize);
        }

        /// <summary>
        /// Gets or creates an async object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policy">The policy to use for creating and resetting objects.</param>
        /// <param name="maxSize">The maximum size of the pool. Defaults to <see cref="ObjectPoolDefaults.GetDefaultMaximumRetained"/>.</param>
        /// <returns>The async object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        public virtual IAsyncObjectPool<T> GetAsyncObjectPool<T>([NotNull] string poolName, IAsyncObjectPoolPolicy<T> policy, int? maxSize = null) where T : class
        {
            Check.NotNullOrWhiteSpace(poolName, nameof(poolName));
            Check.NotNull(policy, nameof(policy));

            return GetAsyncObjectPool<T>(poolName, () => policy, policy.GetType(), maxSize);
        }

        /// <summary>
        /// Gets or creates an async object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policyFactory">The factory to create the policy only when the pool is first created.</param>
        /// <param name="maxSize">The maximum size of the pool. Defaults to <see cref="ObjectPoolDefaults.GetDefaultMaximumRetained"/>.</param>
        /// <returns>The async object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        public virtual IAsyncObjectPool<T> GetAsyncObjectPool<T>([NotNull] string poolName, Func<IAsyncObjectPoolPolicy<T>> policyFactory, int? maxSize = null) where T : class
        {
            Check.NotNullOrWhiteSpace(poolName, nameof(poolName));
            Check.NotNull(policyFactory, nameof(policyFactory));

            return GetAsyncObjectPool<T>(poolName, policyFactory, null, maxSize);
        }

        /// <summary>
        /// Gets or creates an async object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <typeparam name="TPolicy">The type of policy used by the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policyFactory">The factory to create the policy only when the pool is first created.</param>
        /// <param name="maxSize">The maximum size of the pool. Defaults to <see cref="ObjectPoolDefaults.GetDefaultMaximumRetained"/>.</param>
        /// <returns>The async object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        public virtual IAsyncObjectPool<T> GetAsyncObjectPool<T, TPolicy>([NotNull] string poolName, Func<TPolicy> policyFactory, int? maxSize = null)
            where T : class
            where TPolicy : IAsyncObjectPoolPolicy<T>
        {
            Check.NotNullOrWhiteSpace(poolName, nameof(poolName));
            Check.NotNull(policyFactory, nameof(policyFactory));

            return GetAsyncObjectPool<T>(poolName, () => policyFactory(), typeof(TPolicy), maxSize);
        }

        /// <summary>
        /// Removes the pool with the specified name and disposes its retained objects.
        /// </summary>
        public virtual bool RemovePool<T>([NotNull] string poolName) where T : class
        {
            Check.NotNullOrWhiteSpace(poolName, nameof(poolName));

            return RemovePools(new[] { new PoolKey(typeof(T), poolName), new PoolKey(typeof(T), poolName, true) }) > 0;
        }

        /// <summary>
        /// Removes the pool with the specified object type and name, and disposes its retained objects.
        /// </summary>
        public virtual bool RemovePool([NotNull] Type objectType, [NotNull] string poolName)
        {
            Check.NotNull(objectType, nameof(objectType));
            Check.NotNullOrWhiteSpace(poolName, nameof(poolName));

            return RemovePools(new[] { new PoolKey(objectType, poolName), new PoolKey(objectType, poolName, true) }) > 0;
        }

        /// <summary>
        /// Removes all pools and disposes their retained objects.
        /// </summary>
        public virtual int RemoveAll()
        {
            RejectInitializationMutation();
            return RemovePools(PoolMetadatas.Keys);
        }

        /// <summary>
        /// Permanently shuts down this orchestrator and disposes all retained pools.
        /// </summary>
        public virtual void Dispose()
        {
            lock (_lifecycle)
            {
                if (_disposed) return;
                RejectInitializationMutation();
                _disposed = true;
            }
            RemoveAll();
        }

        protected virtual ObjectPool<T> GetPool<T>(
            [NotNull] string poolName,
            Func<IPooledObjectPolicy<T>> policyFactory,
            Type? policyType,
            int? maxSize = null) where T : class
        {
            var effectiveMaxSize = maxSize ?? ObjectPoolDefaults.GetDefaultMaximumRetained();
            var key = new PoolKey(typeof(T), poolName);

            var metadata = GetOrCreateMetadata(
                key,
                () => new Lazy<PoolMetadata>(
                    () =>
                    {
                        var policy = policyFactory();
                        Check.NotNull(policy, nameof(policyFactory));

                        return new PoolMetadata(policyType, effectiveMaxSize, new BoundedObjectPool<T>(policy, effectiveMaxSize));
                    },
                    LazyThreadSafetyMode.ExecutionAndPublication));

            ValidateMetadata<T>(poolName, metadata, policyType, effectiveMaxSize);

            return (ObjectPool<T>)metadata.Pool;
        }

        protected virtual IObjectPool<T> GetObjectPool<T>(
            [NotNull] string poolName,
            Func<IObjectPoolPolicy<T>> policyFactory,
            Type? policyType,
            int? maxSize = null) where T : class
        {
            var pool = GetPool(
                poolName,
                () =>
                {
                    var policy = policyFactory();
                    Check.NotNull(policy, nameof(policyFactory));
                    return new ObjectPoolPolicyAdapter<T>(policy);
                },
                policyType,
                maxSize);

            return new ObjectPoolAdapter<T>(pool);
        }

        protected virtual IAsyncObjectPool<T> GetAsyncObjectPool<T>(
            [NotNull] string poolName,
            Func<IAsyncObjectPoolPolicy<T>> policyFactory,
            Type? policyType,
            int? maxSize = null) where T : class
        {
            var effectiveMaxSize = maxSize ?? ObjectPoolDefaults.GetDefaultMaximumRetained();
            var key = new PoolKey(typeof(T), poolName, true);

            var metadata = GetOrCreateMetadata(
                key,
                () => new Lazy<PoolMetadata>(
                    () =>
                    {
                        var policy = policyFactory();
                        Check.NotNull(policy, nameof(policyFactory));

                        return new PoolMetadata(policyType, effectiveMaxSize, new AsyncObjectPool<T>(policy, effectiveMaxSize));
                    },
                    LazyThreadSafetyMode.ExecutionAndPublication));

            ValidateMetadata<T>(poolName, metadata, policyType, effectiveMaxSize);

            return (IAsyncObjectPool<T>)metadata.Pool;
        }

        protected virtual void ValidateMetadata<T>(
            [NotNull] string poolName,
            [NotNull] PoolMetadata metadata,
            Type? policyType,
            int maxSize) where T : class
        {
            if (policyType != null && metadata.PolicyType != null && metadata.PolicyType != policyType)
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
        }

        protected virtual PoolMetadata GetOrCreateMetadata(PoolKey key, Func<Lazy<PoolMetadata>> metadataFactory)
        {
            Lazy<PoolMetadata> lazy;
            lock (_lifecycle)
            {
                ThrowIfDisposed();
                PoolMetadatas.TryGetValue(key, out lazy!);
                if (lazy != null) _initializing.Add(lazy);
            }
            if (lazy == null)
            {
                var candidate = metadataFactory();
                lock (_lifecycle)
                {
                    ThrowIfDisposed();
                    lazy = PoolMetadatas.GetOrAdd(key, candidate);
                    _initializing.Add(lazy);
                }
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
                var item = new KeyValuePair<PoolKey, Lazy<PoolMetadata>>(key, lazy);
                ((ICollection<KeyValuePair<PoolKey, Lazy<PoolMetadata>>>)PoolMetadatas).Remove(item);
                throw;
            }
            finally
            {
                Volatile.Write(ref scope.Active, 0);
                _initialization.Value = previous;
                lock (_lifecycle) _initializing.Remove(lazy);
            }
        }

        protected virtual bool RemovePoolCore(PoolKey key)
        {
            Lazy<PoolMetadata> lazy;
            bool started;
            lock (_lifecycle)
            {
                if (PoolMetadatas.TryGetValue(key, out var existing)) RejectInitializationMutation(existing);
                if (!PoolMetadatas.TryRemove(key, out lazy!)) return false;
                started = lazy.IsValueCreated || _initializing.Contains(lazy);
            }
            if (started) (lazy.Value.Pool as IDisposable)?.Dispose();
            return true;
        }

        private int RemovePools(IEnumerable<PoolKey> keys)
        {
            // Validate both sync and async entries before removing either one.
            lock (_lifecycle)
            {
                foreach (var key in keys)
                {
                    if (PoolMetadatas.TryGetValue(key, out var lazy)) RejectInitializationMutation(lazy);
                }
            }
            var removed = 0;
            var errors = new List<Exception>();
            foreach (var key in keys)
            {
                try
                {
                    if (RemovePoolCore(key)) removed++;
                }
                catch (Exception error)
                {
                    errors.Add(error);
                }
            }

            if (errors.Count > 0) throw new AggregateException(errors);
            return removed;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().Name);
        }

        private void RejectInitializationMutation(Lazy<PoolMetadata>? target = null)
        {
            // An initialized pool can be retired without waiting on a policy factory.
            if (target?.IsValueCreated == true) return;
            for (var scope = _initialization.Value; scope != null; scope = scope.Parent)
            {
                if (Volatile.Read(ref scope.Active) != 0)
                    throw new InvalidOperationException("Pool lifecycle changes cannot run inside this orchestrator's initialization callback.");
            }
        }
    }
}
