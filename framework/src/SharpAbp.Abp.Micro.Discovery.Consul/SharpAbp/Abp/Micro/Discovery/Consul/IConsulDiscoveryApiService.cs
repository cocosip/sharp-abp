using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public interface IConsulDiscoveryApiService
    {
        Task<List<MicroService>> GetAsync([NotNull] string service, CancellationToken cancellationToken = default);
    }
}
