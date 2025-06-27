using JetBrains.Annotations;
using Microsoft.Extensions.ObjectPool;

namespace SharpAbp.Abp.ObjectPool
{
    public interface IPoolOrchestrator
    {
        ObjectPool<T> GetPool<T>([NotNull] string poolName, IPooledObjectPolicy<T> policy, int? maxSize = null) where T : class;
    }
}
