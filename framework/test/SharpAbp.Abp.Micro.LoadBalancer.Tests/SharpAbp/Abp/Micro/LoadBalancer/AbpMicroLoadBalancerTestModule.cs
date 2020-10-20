using SharpAbp.Abp.Micro.Discovery.AddressTable;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    [DependsOn(
         typeof(AbpMicroLoadBalancerModule),
         typeof(AbpMicroDiscoveryAddressTableModule),
         typeof(AbpTestBaseModule),
         typeof(AbpAutofacModule)
         )]
    public class AbpMicroLoadBalancerTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMicroLoadBalancerOptions>(options =>
            {
                options.Configurations
                    .ConfigureDefault(balancer =>
                    {
                        balancer.UseRandom(c =>
                        {
                            c.Seed = 123456;
                        });
                    })
                    .Configure("service1", balancer =>
                    {
                        balancer.UseNoLoadbalancer(c =>
                        {
                            c.FirstOne = true;
                        });
                    })
                    .Configure("service2", balancer =>
                    {
                        balancer.UseRoundRobin(c =>
                        {
                            c.Step = 1;
                        });
                    })
                    .Configure("service3", balancer =>
                    {
                        balancer.UseWeightRoundRobin(c =>
                        {
                            c.Weights = "127.0.0.1:100-3,127.0.0.1:101-4,127.0.0.102-5";
                        });
                    });

            });
        }
    }
}
