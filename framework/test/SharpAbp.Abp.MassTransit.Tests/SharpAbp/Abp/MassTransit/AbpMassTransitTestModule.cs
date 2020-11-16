using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Options;
using System;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace SharpAbp.Abp.MassTransit
{
    [DependsOn(
      typeof(AbpMassTransitModule),
      typeof(AbpTestBaseModule),
      typeof(AbpAutofacModule)
    )]
    public class AbpMassTransitTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

            Configure<MassTransitOptions>(options =>
            {
                options.BusConfigurator = new Action<IServiceCollectionBusConfigurator>(x =>
                {
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.TransportConcurrencyLimit = 100;
                        cfg.ConfigureEndpoints(context);
                    });
                });
            });

        }

       
    }
}
