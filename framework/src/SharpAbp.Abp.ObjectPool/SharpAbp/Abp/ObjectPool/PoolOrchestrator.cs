using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;
using Microsoft.Extensions.ObjectPool;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.ObjectPool
{
    public class PoolOrchestrator : IPoolOrchestrator, ISingletonDependency
    {
        protected ObjectPoolProvider Provider { get; }
        protected ConcurrentDictionary<string, object> Pools { get; }

        public PoolOrchestrator(ObjectPoolProvider provider)
        {
            Provider = provider;
            Pools = new ConcurrentDictionary<string, object>();
        }

        public virtual ObjectPool<T> GetPool<T>([NotNull] string poolName, IPooledObjectPolicy<T> policy, int? maxSize = null) where T : class
        {
            Check.NotNullOrWhiteSpace(poolName, nameof(poolName));
            Check.NotNull(policy, nameof(policy));
            var key = NormalizeKey(typeof(T), poolName);
            return (ObjectPool<T>)Pools.GetOrAdd(key, _ =>
            {
                // 动态创建带大小控制的Provider
                var configurableObjectPoolProvider = new ConfigurableObjectPoolProvider
                {
                    MaxSize = maxSize ?? Provider.GetDefaultSize()
                };
                return configurableObjectPoolProvider.Create(policy);
            });
        }

        protected virtual string NormalizeKey(Type t, string poolName)
        {
            return $"{t.FullName}-{poolName}"; ;
        }
    }
}
