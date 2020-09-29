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
                var entries = FilterByTags(addressTableService.Entries, args.Tags);
                if (entries != null && entries.Any())
                {
                    var microServices = entries
                        .Select(x => x.ToMicroService(args.Service))
                        .ToList();
                    return Task.FromResult(microServices);
                }
            }
            return Task.FromResult(new List<MicroService>());
        }


        protected virtual List<AddressTableServiceEntry> FilterByTags(List<AddressTableServiceEntry> entries, List<string> tags)
        {
            if (tags == null || !tags.Any())
            {
                return entries;
            }

            if (entries.Any())
            {
                return entries.Where(x => tags.All(x.Tags.Contains)).ToList();
            }
            return new List<AddressTableServiceEntry>();
        }

    }
}
