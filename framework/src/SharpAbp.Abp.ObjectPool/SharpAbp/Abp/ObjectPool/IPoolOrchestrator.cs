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
    }
}
