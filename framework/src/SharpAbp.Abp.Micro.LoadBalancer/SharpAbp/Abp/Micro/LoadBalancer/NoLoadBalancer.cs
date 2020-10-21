using SharpAbp.Abp.Micro.Discovery;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class NoLoadBalancer : ILoadBalancer
    {
        public string BalancerType => LoadBalancerConsts.NoLoadBalancer;

        public string Service { get; }

        protected NoLoadBalancerConfiguration Configuration { get; }
        protected IServiceDiscoveryProvider DiscoveryProvider { get; }

        public NoLoadBalancer(string service, NoLoadBalancerConfiguration configuration, IServiceDiscoveryProvider discoveryProvider)
        {
            Service = service;
            Configuration = configuration;
            DiscoveryProvider = discoveryProvider;
        }

        public virtual async Task<MicroService> Lease(string tag = "", CancellationToken cancellationToken = default)
        {
            var services = await DiscoveryProvider.GetServices(Service, tag, cancellationToken);
            if (services == null || services.Count == 0)
            {
                throw new AbpException($"There were no services in NoLoadBalancer '{Service}'");
            }
            var service = Configuration.FirstOne ? services.FirstOrDefault() : services.LastOrDefault();
            return service;
        }

    }
}
