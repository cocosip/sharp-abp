using Consul;
using JetBrains.Annotations;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public interface IConsulClientFactory
    {
        /// <summary>
        /// Get a consul client
        /// </summary>
        /// <returns></returns>
        [NotNull]
        IConsulClient Get();
    }
}
