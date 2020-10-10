using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class DefaultConsulDiscoverer : IConsulDiscoverer, ITransientDependency
    {
        protected IConsulDiscoveryApiService ConsulDiscoveryApiService { get; }

        public DefaultConsulDiscoverer(IConsulDiscoveryApiService consulDiscoveryApiService)
        {
            ConsulDiscoveryApiService = consulDiscoveryApiService;
        }

        public virtual async Task<List<MicroService>> GetAsync([NotNull] string service, CancellationToken cancellationToken = default)
        {
            return await ConsulDiscoveryApiService.GetAsync(service, cancellationToken);
        }

    }
}
