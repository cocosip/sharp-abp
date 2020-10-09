using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AddressTableConfiguration
    {
        public string Service { get; set; }

        public List<ServiceAddressEntry> Entries { get; }

        public AddressTableConfiguration()
        {
            Entries = new List<ServiceAddressEntry>();
        }

        public AddressTableConfiguration([NotNull] string service) : this()
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));
        }

        public AddressTableConfiguration AddIfNotContains([NotNull] ServiceAddressEntry entry)
        {
            Entries.AddIfNotContains(x => x.Id == entry.Id, () => entry);
            return this;
        }

        public AddressTableConfiguration Remove([NotNull] string id)
        {
            var entry = Entries.FirstOrDefault(x => x.Id == id);
            if (entry != null)
            {
                return Remove(entry);
            }
            return this;
        }

        public AddressTableConfiguration Remove([NotNull] ServiceAddressEntry entry)
        {
            Entries.Remove(entry);
            return this;
        }

    }
}
