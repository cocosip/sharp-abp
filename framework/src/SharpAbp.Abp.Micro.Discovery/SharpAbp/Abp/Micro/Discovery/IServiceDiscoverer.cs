using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Micro.Discovery
{
    public interface IServiceDiscoverer<T> : IServiceDiscoverer
        where T : class
    {


    }

    public interface IServiceDiscoverer
    {
        /// <summary>
        /// Get current discoverer configuration
        /// </summary>
        /// <returns></returns>
        DiscoveryConfiguration GetConfiguration();

        /// <summary>
        /// Get service entry list by service name and tags
        /// </summary>
        /// <param name="service">Service name</param>
        /// <param name="tags">Tags</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<MicroService>> GetAsync(string service, List<string> tags = default, CancellationToken cancellationToken = default);

    }
}
