using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Micro.Discovery.TestObjects
{
    public class Test2ServiceDiscoveryProvider : IServiceDiscoveryProvider
    {
        public Task<List<MicroService>> GetAsync(string service, string tag = "", CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
