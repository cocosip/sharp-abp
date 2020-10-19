namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class AbpMicroLoadBalancerOptions
    {
        public LoadBalancerConfigurations Configurations { get; }

        public AbpMicroLoadBalancerOptions()
        {
            Configurations = new LoadBalancerConfigurations();
        }

    }
}
