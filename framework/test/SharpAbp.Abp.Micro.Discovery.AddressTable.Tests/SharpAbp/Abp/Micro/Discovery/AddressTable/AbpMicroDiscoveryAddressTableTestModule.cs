using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    [DependsOn(
         typeof(AbpMicroDiscoveryAddressTableModule),
         typeof(AbpTestBaseModule),
         typeof(AbpAutofacModule)
         )]
    public class AbpMicroDiscoveryAddressTableTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            Configure<AbpMicroDiscoveryAddressTableOptions>(options =>
            {
                options.Configure(configuration.GetSection("AddressTable"));
            });

        }
    }
}
