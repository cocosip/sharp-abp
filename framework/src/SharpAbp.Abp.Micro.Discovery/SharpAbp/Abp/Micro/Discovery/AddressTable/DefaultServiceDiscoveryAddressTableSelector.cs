using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class DefaultServiceDiscoveryAddressTableSelector : IServiceDiscoveryAddressTableSelector, ITransientDependency
    {
        protected AddressTableDiscoveryOptions Options { get; }

        public DefaultServiceDiscoveryAddressTableSelector(IOptions<AddressTableDiscoveryOptions> options)
        {
            Options = options.Value;
        }

        public virtual AddressTableService Get(string service)
        {
            return Options.GetService(service);
        }

    }
}
