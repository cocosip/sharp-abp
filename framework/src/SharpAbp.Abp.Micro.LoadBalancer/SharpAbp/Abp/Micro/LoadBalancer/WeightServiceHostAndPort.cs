using System;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class WeightServiceHostAndPort : IEquatable<WeightServiceHostAndPort>
    {
        public ServiceHostAndPort HostAndPort { get; set; }

        public int Weight { get; set; }

        public WeightServiceHostAndPort()
        {

        }

        public WeightServiceHostAndPort(ServiceHostAndPort hostAndPort, int weight)
        {
            HostAndPort = hostAndPort;
            Weight = weight;
        }

        public bool Equals(WeightServiceHostAndPort other)
        {
            return HostAndPort == other.HostAndPort && Weight == other.Weight;
        }


        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is WeightServiceHostAndPort other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.InvariantCulture.GetHashCode(HostAndPort) | Weight.GetHashCode();
        }

       
    }
}
