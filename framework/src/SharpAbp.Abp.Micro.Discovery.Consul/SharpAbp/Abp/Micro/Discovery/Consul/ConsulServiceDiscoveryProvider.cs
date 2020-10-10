using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class ConsulServiceDiscoveryProvider : IServiceDiscoveryProvider, ITransientDependency
    {
        protected AbpMicroDiscoveryConsulOptions Options { get; }
        protected IConsulDiscovererFactory ConsulDiscovererFactory { get; }
        public ConsulServiceDiscoveryProvider(IOptions<AbpMicroDiscoveryConsulOptions> options, IConsulDiscovererFactory consulDiscovererFactory)
        {
            Options = options.Value;
            ConsulDiscovererFactory = consulDiscovererFactory;
        }


        public virtual async Task<List<MicroService>> GetAsync([NotNull] string service, string tag = "", CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));

            var consulDiscoverer = ConsulDiscovererFactory.GetDiscoverer(service);

            var services = await consulDiscoverer.GetAsync(service, cancellationToken);
            services = FilterByTag(services, tag);
            return services;
        }


        private List<MicroService> FilterByTag(List<MicroService> services, string tag)
        {
            if (!tag.IsNullOrWhiteSpace())
            {
                return services.Where(x => x.Tags.Contains(tag)).ToList();
            }

            return services;
        }

    }
}
