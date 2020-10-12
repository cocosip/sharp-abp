using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Micro.Discovery
{
    public interface IServiceDiscoveryProvider
    {
        Task<List<MicroService>> GetServices(string service, string tag = "", CancellationToken cancellationToken = default);
    }
}
