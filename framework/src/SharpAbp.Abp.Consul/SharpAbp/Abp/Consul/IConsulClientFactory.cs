using Consul;
using JetBrains.Annotations;

namespace SharpAbp.Abp.Consul
{
    public interface IConsulClientFactory
    {
        /// <summary>
        /// Get consul client by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns></returns>
        [NotNull]
        IConsulClient Get([NotNull] string name = DefaultConsul.Name);
    }
}
