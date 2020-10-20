namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class AbpMicroLoadBalancerOptions
    {
        public LoadBalancerConfigurations Configurations { get; }

        public LoadBalancerProviderConfigurations Providers { get; }

        public AbpMicroLoadBalancerOptions()
        {
            Configurations = new LoadBalancerConfigurations();
            Providers = new LoadBalancerProviderConfigurations();
        }

    }
}
