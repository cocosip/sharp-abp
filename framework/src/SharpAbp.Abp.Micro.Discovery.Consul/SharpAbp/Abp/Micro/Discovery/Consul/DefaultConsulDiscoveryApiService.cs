using Consul;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class DefaultConsulDiscoveryApiService : IConsulDiscoveryApiService, ITransientDependency
    {
        protected IConsulClient Client { get; }

        public DefaultConsulDiscoveryApiService(IConsulClientFactory consulClientFactory)
        {
            Client = consulClientFactory.Get();
        }

        public virtual async Task<List<MicroService>> GetAsync([NotNull] string service, CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));

            var queryResult = await Client.Health.Service(service, string.Empty, true, ct: cancellationToken);
            var services = new List<MicroService>();

            foreach (var serviceEntry in queryResult.Response)
            {
                var nodes = await Client.Catalog.Nodes(cancellationToken);
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
                Host = serviceNode == null ? serviceEntry.Service.Address : serviceNode.Name,
                Port = serviceEntry.Service.Port,
                Tags = serviceEntry.Service.Tags?.ToList() ?? new List<string>(),
                Version = GetVersionFromStrings(serviceEntry.Service.Tags)
            };
        }

        private string GetVersionFromStrings(IEnumerable<string> strings)
        {
            return strings
                ?.FirstOrDefault(x => x.StartsWith(AbpMicroDiscoveryConsts.VersionPrefix, StringComparison.Ordinal))
                .TrimStart(AbpMicroDiscoveryConsts.VersionPrefix.ToCharArray());
        }

    }
}
