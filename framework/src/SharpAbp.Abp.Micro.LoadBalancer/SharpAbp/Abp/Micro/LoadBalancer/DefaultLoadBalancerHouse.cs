using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class DefaultLoadBalancerHouse : ILoadBalancerHouse, ISingletonDependency
    {
        protected ILogger Logger { get; }
        protected ILoadBalancerFactory LoadBalancerFactory { get; }

        private readonly object SyncObject = new object();
        private readonly ConcurrentDictionary<string, ILoadBalancer> _loadBalancers;
        public DefaultLoadBalancerHouse(ILogger<DefaultLoadBalancerHouse> logger, ILoadBalancerFactory loadBalancerFactory)
        {
            Logger = logger;
            LoadBalancerFactory = loadBalancerFactory;
            _loadBalancers = new ConcurrentDictionary<string, ILoadBalancer>();
        }

        public virtual ILoadBalancer Get([NotNull] string service)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));

            if (!_loadBalancers.TryGetValue(service, out ILoadBalancer loadBalancer))
            {
                lock (SyncObject)
                {
                    if (!_loadBalancers.TryGetValue(service, out loadBalancer))
                    {
                        Logger.LogDebug("Can't find loadbalancer by service :'{0}',will create loadbalancer.", service);

                        //create load balancer by service name
                        loadBalancer = LoadBalancerFactory.Get(service);
                        if (loadBalancer != null)
                        {
                            if (!_loadBalancers.TryAdd(service, loadBalancer))
                            {
                                Logger.LogDebug("Can't add loadbalancer to dict!");
                            }
                        }
                    }
                }
            }

            if (loadBalancer == null)
            {
                throw new ArgumentException($"Could not find any loadbalancer by service {service}");
            }

            return loadBalancer;
        }

    }
}
