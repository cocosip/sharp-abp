using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Micro.Discovery.TestObjects;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.Discovery
{
    [DependsOn(
        typeof(AbpMicroDiscoveryModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
        )]
    public class AbpMicroDiscoveryTestModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMicroDiscoveryOptions>(options =>
            {
                options.ProviderNameMappers.SetProvider<Test1ServiceDiscoveryProvider>("test1");
                options.ProviderNameMappers.SetProvider<Test2ServiceDiscoveryProvider>("test2");
                options.ProviderNameMappers.SetProvider<Test3ServiceDiscoveryProvider>("test3");
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<IServiceDiscoveryProvider, Test1ServiceDiscoveryProvider>();
            context.Services.AddTransient<IServiceDiscoveryProvider, Test2ServiceDiscoveryProvider>();

            Configure<AbpMicroDiscoveryOptions>(options =>
            {
                options.Configure(context.Services.GetConfiguration().GetSection("Services"));
            });
        }
    }
}
