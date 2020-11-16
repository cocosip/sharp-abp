using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MassTransit
{

    /// <summary>
    /// MassTransit module
    /// </summary>
    public class AbpMassTransitModule : AbpModule
    {

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            var configure = context.Services.GetSingletonInstance<IConfigureOptions<MassTransitOptions>>();
            //var options = Options.Create<MassTransitOptions>(new MassTransitOptions());
            var massTransitOptions = new MassTransitOptions();

            configure.Configure(massTransitOptions);

            if (massTransitOptions.BusConfigurator != null)
            {
                context.Services.AddMassTransit(massTransitOptions.BusConfigurator);
            }
            else if (massTransitOptions.MediatorConfigurator != null)
            {
                context.Services.AddMediator(massTransitOptions.MediatorConfigurator);
            }

            context.Services.AddMassTransitHostedService();
        }


        public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
        {
            var options = context.ServiceProvider.GetService<IOptions<MassTransitOptions>>().Value;
        }

    }
}
