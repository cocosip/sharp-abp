using SharpAbp.Abp.Micro.Discovery;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class RandomLoadBalancer : ILoadBalancer
    {
        public string BalancerType => LoadBalancerConsts.Random;

        public string Service { get; }

        protected RandomLoadBalancerConfiguration Configuration { get; }
        protected IServiceDiscoveryProvider DiscoveryProvider { get; }

        private readonly object SyncObject = new object();

        public RandomLoadBalancer(string service, RandomLoadBalancerConfiguration configuration, IServiceDiscoveryProvider discoveryProvider)
        {
            DiscoveryProvider = discoveryProvider;
            Configuration = configuration;
            Service = service;
        }

        public virtual async Task<MicroService> Lease(string tag = "", CancellationToken cancellationToken = default)
        {
            var services = await DiscoveryProvider.GetServices(Service, tag, cancellationToken);
            if (services == null || services.Count == 0)
            {
                throw new AbpException($"There were no services in RandomLoadBalancer '{Service}'");
            }

            lock (SyncObject)
            {
                var rd = new Random();
                var index = rd.Next(services.Count);
                return services[index];
            }
        }

    }
}
