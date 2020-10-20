using SharpAbp.Abp.Micro.Discovery;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class RoundRobinLoadBalancer : ILoadBalancer
    {
        public string BalancerType => LoadBalancerConsts.RoundRobin;

        public string Service { get; }

        protected RoundRobinLoadBalancerConfiguration Configugration { get; }
        protected IServiceDiscoveryProvider DiscoveryProvider { get; }

        private readonly object SyncObject = new object();
        private int _sequence = 0;

        public RoundRobinLoadBalancer(string service, RoundRobinLoadBalancerConfiguration configuration, IServiceDiscoveryProvider discoveryProvider)
        {
            Service = service;
            Configugration = configuration;
            DiscoveryProvider = discoveryProvider;
        }

        public virtual async Task<MicroService> Lease(string tag = "", CancellationToken cancellationToken = default)
        {
            var services = await DiscoveryProvider.GetServices(Service, tag, cancellationToken);
            if (services == null || services.Count == 0)
            {
                throw new AbpException($"There were no services in RoundRobinLoadBalancer '{Service}'");
            }
            lock (SyncObject)
            {
                var sequence = Interlocked.Exchange(ref _sequence, _sequence + Configugration.Step);
                var index = sequence % services.Count;
                return services[index];
            }
        }

    }
}
