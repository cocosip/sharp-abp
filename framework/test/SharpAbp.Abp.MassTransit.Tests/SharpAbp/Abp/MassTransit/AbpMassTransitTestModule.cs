using MassTransit;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

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
            context.Services
                .AddMassTransit(x =>
                {
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.TransportConcurrencyLimit = 100;
                        cfg.ConfigureEndpoints(context);
                    });

                    //x.AddConsumer<SubmitOrderConsumer>();
                });
        }


        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMassTransitHostedService();
        }

    }
}
