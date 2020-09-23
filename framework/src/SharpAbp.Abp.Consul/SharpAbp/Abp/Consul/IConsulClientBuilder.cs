using Consul;

namespace SharpAbp.Abp.Consul
{
    public interface IConsulClientBuilder
    {
        /// <summary>
        /// Create consul client
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        IConsulClient CreateClient(ConsulConfiguration configuration);
    }
}
