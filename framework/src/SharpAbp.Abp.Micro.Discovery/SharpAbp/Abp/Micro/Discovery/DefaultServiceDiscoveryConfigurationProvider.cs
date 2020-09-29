using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class DefaultServiceDiscoveryConfigurationProvider : IServiceDiscoveryConfigurationProvider, ITransientDependency
    {
        protected AbpMicroDiscoveryOptions Options { get; }

        public DefaultServiceDiscoveryConfigurationProvider(IOptions<AbpMicroDiscoveryOptions> options)
        {
            Options = options.Value;
        }

        public virtual ServiceDiscoveryConfiguration Get(string name)
        {
            return Options.Configurations.GetConfiguration(name);
        }


    }
}
