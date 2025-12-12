using Microsoft.Extensions.ObjectPool;

namespace SharpAbp.Abp.ObjectPool
{
    /// <summary>
    /// An object pool provider that allows configuration of the maximum pool size.
    /// This class extends <see cref="DefaultObjectPoolProvider"/> to support custom maximum retained objects.
    /// </summary>
    /// <remarks>
    /// This provider can be used when you need to create pools with specific size limits.
    /// If MaxSize is not set, it behaves like the default provider.
    /// </remarks>
    public class ConfigurableObjectPoolProvider : DefaultObjectPoolProvider
    {
        /// <summary>
        /// Gets or sets the maximum number of objects to retain in the pool.
        /// If not set, the default behavior of <see cref="DefaultObjectPoolProvider"/> is used.
        /// </summary>
        public int? MaxSize { get; set; }

        /// <summary>
        /// Creates an object pool for the specified policy.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="policy">The policy to use for creating and managing pooled objects.</param>
        /// <returns>An object pool configured with the specified maximum size, or the default pool if MaxSize is not set.</returns>
        public override ObjectPool<T> Create<T>(IPooledObjectPolicy<T> policy)
        {
            return MaxSize.HasValue
                ? new DefaultObjectPool<T>(policy, MaxSize.Value)
                : base.Create(policy);
        }
    }
}
