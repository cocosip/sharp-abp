using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.Micro
{
    public class MicroService : IEquatable<MicroService>
    {
        public string Id { get; set; }

        public string Service { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }

        public List<string> Tags { get; set; }

        public MicroService()
        {
            Tags = new List<string>();
        }


        public bool Equals(MicroService other)
        {
            return Id == other.Id && Service == other.Service && Address == other.Address && Port == other.Port;
        }


        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is MicroService other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.InvariantCulture.GetHashCode(Id)
                | StringComparer.InvariantCulture.GetHashCode(Service)
                | StringComparer.InvariantCulture.GetHashCode(Address)
                | Port.GetHashCode();
        }

        public static bool operator ==(MicroService s1, MicroService s2) => s1.Equals(s2);

        public static bool operator !=(MicroService s1, MicroService s2) => !s1.Equals(s2);
    }
}
