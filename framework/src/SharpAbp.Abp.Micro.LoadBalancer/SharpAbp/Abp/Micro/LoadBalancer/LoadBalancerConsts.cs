namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class LoadBalancerConsts
    {
        public const string DefaultService = "default";

        public const string NoLoadBalancer = "NoLoadBalancer";

        public const string Random = "Random";

        public const string RoundRobin = "RoundRobin";

        public const string WeightRoundRobin = "WeightRoundRobin";

    }


    public class NoBalancerConfigurationNames
    {
        public const string FirstOne = "NoBalancer.FirstOne";
    }
    public class RandomLoadBalancerConfigurationNames
    {
        public const string Seed = "Random.Seed";
    }

    public class RoundRobinLoadBalancerConfigurationNames
    {
        public const string Step = "RoundRobin.Step";
    }

    public class WeightRoundRobinLoadBalancerConfigurationNames
    {
        public const string Weights = "WeightRoundRobin.Weights";
    }

}
