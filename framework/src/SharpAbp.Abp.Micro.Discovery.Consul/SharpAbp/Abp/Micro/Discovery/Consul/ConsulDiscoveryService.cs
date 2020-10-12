using Consul;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class ConsulDiscoveryService : IConsulDiscoveryService, ITransientDependency
    {
        protected IConsulClientFactory ConsulClientFactory { get; }
        public ConsulDiscoveryService(IConsulClientFactory consulClientFactory)
        {
            ConsulClientFactory = consulClientFactory;
        }

        public virtual async Task<List<MicroService>> GetAsync([NotNull] string service, CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));
            //Get client
            var client = ConsulClientFactory.Get();

            var queryResult = await client.Health.Service(service, string.Empty, true, ct: cancellationToken);
            var services = new List<MicroService>();

            foreach (var serviceEntry in queryResult.Response)
            {
                var nodes = await client.Catalog.Nodes(cancellationToken);
                if (nodes.Response == null)
                {
                    services.Add(BuildService(serviceEntry, null));
                }
                else
                {
                    var serviceNode = nodes.Response.FirstOrDefault(n => n.Address == serviceEntry.Service.Address);
                    services.Add(BuildService(serviceEntry, serviceNode));
                }
            }

            return services;
        }

        private MicroService BuildService(ServiceEntry serviceEntry, Node serviceNode)
        {
            return new MicroService()
            {
                Id = serviceEntry.Service.ID,
                Service = serviceEntry.Service.Service,
                Address = serviceNode == null ? serviceEntry.Service.Address : serviceNode.Name,
                Port = serviceEntry.Service.Port,
                Tags = serviceEntry.Service.Tags?.ToList() ?? new List<string>()
            };
        }

    }
}
