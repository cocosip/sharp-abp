using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AddressTableConfiguration
    {
        public string Service { get; set; }

        public List<AddressTableEntry> Entries { get; set; }

        public AddressTableConfiguration()
        {
            Entries = new List<AddressTableEntry>();
        }


        public AddressTableConfiguration AddIfNotContains([NotNull] AddressTableEntry entry)
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

        public AddressTableConfiguration Remove([NotNull] AddressTableEntry entry)
        {
            Entries.Remove(entry);
            return this;
        }

    }
}
