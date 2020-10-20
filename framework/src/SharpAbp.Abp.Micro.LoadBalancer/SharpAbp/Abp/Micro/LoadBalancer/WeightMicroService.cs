using System;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class WeightMicroService : IComparable<WeightMicroService>
    {
        public int Weight { get; }

        public MicroService Service { get; }

        public int CurrentWeight { get; set; }

        public WeightMicroService(int weight, MicroService service, int currentWeight)
        {
            Weight = weight;
            Service = service;
            CurrentWeight = currentWeight;
        }

        public int CompareTo(WeightMicroService other)
        {
            return CurrentWeight - other.CurrentWeight;
        }


        public static bool operator >(WeightMicroService s1, WeightMicroService s2) => s1.CompareTo(s2) > 0;
        public static bool operator <(WeightMicroService s1, WeightMicroService s2) => s1.CompareTo(s2) < 0;
    }
}
