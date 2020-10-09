using Consul;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class DefaultConsulDiscoverer : IConsulDiscoverer, ITransientDependency
    {
        protected IConsulClient Client { get; }

        public DefaultConsulDiscoverer(IConsulClientFactory consulClientFactory)
        {
            Client = consulClientFactory.Get();
        }

        public virtual async Task<List<ServiceEntry>> GetAsync([NotNull] string service)
        {
            var queryResult = await Client.Health.Service(service, string.Empty, true);
            var services = new List<ServiceEntry>();

            //foreach (var serviceEntry in queryResult.Response)
            //{
            //    var nodes = await Client.Catalog.Nodes();
            //    if (nodes.Response == null)
            //    {
            //        services.Add(BuildService(serviceEntry, null));
            //    }
            //    else
            //    {
            //        var serviceNode = nodes.Response.FirstOrDefault(n => n.Address == serviceEntry.Service.Address);
            //        services.Add(BuildService(serviceEntry, serviceNode));
            //    }
            //}

            return default;
        }
    }
}
