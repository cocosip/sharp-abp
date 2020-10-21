using Microsoft.Extensions.DependencyInjection;
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
            Configure<AbpMicroDiscoveryAddressTableOptions>(options =>
            {
                var configuration = context.Services.GetConfiguration().GetSection("AddressTable");
                options.Configure(configuration);
            });

            Configure<AbpMicroLoadBalancerOptions>(options =>
            {
                var configuration = context.Services.GetConfiguration().GetSection("LoadBalancers");
                options.Configure(configuration);
            });

        }
    }
}
