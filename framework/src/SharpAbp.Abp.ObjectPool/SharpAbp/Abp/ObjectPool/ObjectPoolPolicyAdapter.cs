using Microsoft.Extensions.ObjectPool;

namespace SharpAbp.Abp.ObjectPool
{
    public class ObjectPoolPolicyAdapter<T> : IPooledObjectPolicy<T>
        where T : class
    {
        protected IObjectPoolPolicy<T> Policy { get; }

        public ObjectPoolPolicyAdapter(IObjectPoolPolicy<T> policy)
        {
            Policy = policy;
        }

        public virtual T Create()
        {
            return Policy.Create();
        }

        public virtual bool Return(T obj)
        {
            return Policy.Return(obj);
        }
    }
}
