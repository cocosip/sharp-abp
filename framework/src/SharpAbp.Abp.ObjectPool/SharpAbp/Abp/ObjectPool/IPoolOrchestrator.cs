using System;
using JetBrains.Annotations;
using Microsoft.Extensions.ObjectPool;

namespace SharpAbp.Abp.ObjectPool
{
    public interface IPoolOrchestrator
    {
        /// <summary>
        /// Gets or creates an object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policy">The policy to use for creating and resetting objects.</param>
        /// <param name="maxSize">The maximum size of the pool.</param>
        /// <returns>The object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        ObjectPool<T> GetPool<T>([NotNull] string poolName, IPooledObjectPolicy<T> policy, int? maxSize = null) where T : class;

        /// <summary>
        /// Gets or creates an object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policyFactory">The factory to create the policy only when the pool is first created.</param>
        /// <param name="maxSize">The maximum size of the pool.</param>
        /// <returns>The object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        ObjectPool<T> GetPool<T>([NotNull] string poolName, Func<IPooledObjectPolicy<T>> policyFactory, int? maxSize = null) where T : class;

        /// <summary>
        /// Gets or creates an object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <typeparam name="TPolicy">The type of policy used by the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policyFactory">The factory to create the policy only when the pool is first created.</param>
        /// <param name="maxSize">The maximum size of the pool.</param>
        /// <returns>The object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        ObjectPool<T> GetPool<T, TPolicy>([NotNull] string poolName, Func<TPolicy> policyFactory, int? maxSize = null)
            where T : class
            where TPolicy : IPooledObjectPolicy<T>;

        /// <summary>
        /// Gets or creates an object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policy">The policy to use for creating and resetting objects.</param>
        /// <param name="maxSize">The maximum size of the pool.</param>
        /// <returns>The object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        IObjectPool<T> GetObjectPool<T>([NotNull] string poolName, IObjectPoolPolicy<T> policy, int? maxSize = null) where T : class;

        /// <summary>
        /// Gets or creates an object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policyFactory">The factory to create the policy only when the pool is first created.</param>
        /// <param name="maxSize">The maximum size of the pool.</param>
        /// <returns>The object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        IObjectPool<T> GetObjectPool<T>([NotNull] string poolName, Func<IObjectPoolPolicy<T>> policyFactory, int? maxSize = null) where T : class;

        /// <summary>
        /// Gets or creates an object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <typeparam name="TPolicy">The type of policy used by the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policyFactory">The factory to create the policy only when the pool is first created.</param>
        /// <param name="maxSize">The maximum size of the pool.</param>
        /// <returns>The object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        IObjectPool<T> GetObjectPool<T, TPolicy>([NotNull] string poolName, Func<TPolicy> policyFactory, int? maxSize = null)
            where T : class
            where TPolicy : IObjectPoolPolicy<T>;

        /// <summary>
        /// Gets or creates an async object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policy">The policy to use for creating and resetting objects.</param>
        /// <param name="maxSize">The maximum size of the pool.</param>
        /// <returns>The async object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        IAsyncObjectPool<T> GetAsyncObjectPool<T>([NotNull] string poolName, IAsyncObjectPoolPolicy<T> policy, int? maxSize = null) where T : class;

        /// <summary>
        /// Gets or creates an async object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policyFactory">The factory to create the policy only when the pool is first created.</param>
        /// <param name="maxSize">The maximum size of the pool.</param>
        /// <returns>The async object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        IAsyncObjectPool<T> GetAsyncObjectPool<T>([NotNull] string poolName, Func<IAsyncObjectPoolPolicy<T>> policyFactory, int? maxSize = null) where T : class;

        /// <summary>
        /// Gets or creates an async object pool for the specified type and name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <typeparam name="TPolicy">The type of policy used by the pool.</typeparam>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="policyFactory">The factory to create the policy only when the pool is first created.</param>
        /// <param name="maxSize">The maximum size of the pool.</param>
        /// <returns>The async object pool.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to get a pool with different parameters than previously used.</exception>
        IAsyncObjectPool<T> GetAsyncObjectPool<T, TPolicy>([NotNull] string poolName, Func<TPolicy> policyFactory, int? maxSize = null)
            where T : class
            where TPolicy : IAsyncObjectPoolPolicy<T>;
    }
}
