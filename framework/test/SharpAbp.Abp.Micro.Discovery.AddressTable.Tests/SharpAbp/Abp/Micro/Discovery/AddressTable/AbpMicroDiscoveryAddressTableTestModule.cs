using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Micro.Discovery.AddressTable.TestObjects;
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
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMicroDiscoveryOptions>(options =>
            {
                options.ProviderNameMappers.SetProvider("test1", typeof(Test1ServiceDiscoveryProvider));
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<IServiceDiscoveryProvider, Test1ServiceDiscoveryProvider>();

            var configuration = context.Services.GetConfiguration();

            Configure<AbpMicroDiscoveryOptions>(options =>
            {
                options.Configurations.ConfigureDefault(c =>
                {
                    c.UseAddressTable();
                });
            });


            Configure<AbpMicroDiscoveryOptions>(options =>
            {
                options.Configure(configuration.GetSection("Services"));
            });


            Configure<AbpMicroDiscoveryAddressTableOptions>(options =>
            {
                options.Configure(configuration.GetSection("AddressTable"));
            });

        }
    }
}
