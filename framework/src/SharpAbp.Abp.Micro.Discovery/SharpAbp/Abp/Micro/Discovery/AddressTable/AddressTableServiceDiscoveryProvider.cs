using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AddressTableServiceDiscoveryProvider : IServiceDiscoveryProvider, ITransientDependency
    {
        protected IServiceDiscoveryAddressTableSelector AddressTableSelector { get; }
        public AddressTableServiceDiscoveryProvider(IServiceDiscoveryAddressTableSelector addressTableSelector)
        {
            AddressTableSelector = addressTableSelector;
        }

        public virtual Task<List<MicroService>> GetAsync(ServiceDiscoveryProviderGetArgs args)
        {
            Check.NotNullOrWhiteSpace(args.Service, nameof(args.Service));

            var addressTableService = AddressTableSelector.Get(args.Service);

            if (addressTableService != null)
            {
                var entries = addressTableService.Entries
                    .WhereIf(!args.Tag.IsNullOrWhiteSpace(), x => TagMatch(args.Tag, x.Tags))
                    .ToList();
                if (entries.Any())
                {
                    var microServices = entries
                        .Select(x => x.ToMicroService(args.Service))
                        .ToList();
                    return Task.FromResult(microServices);
                }
            }
            return Task.FromResult(new List<MicroService>());
        }


        protected virtual bool TagMatch(string tag, List<string> tags)
        {
            return tags.Any(x => tag.Equals(x, StringComparison.OrdinalIgnoreCase));
        }

    }
}
