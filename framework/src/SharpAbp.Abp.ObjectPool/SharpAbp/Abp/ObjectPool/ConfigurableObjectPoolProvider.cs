using Microsoft.Extensions.ObjectPool;

namespace SharpAbp.Abp.ObjectPool
{
    public class ConfigurableObjectPoolProvider : DefaultObjectPoolProvider
    {
        public int? MaxSize { get; set; }

        public override ObjectPool<T> Create<T>(IPooledObjectPolicy<T> policy)
        {
            return MaxSize.HasValue ? new DefaultObjectPool<T>(policy, MaxSize.Value) : base.Create(policy);
        }
    }
}
