using MassTransit;
using Volo.Abp.Modularity;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SharpAbp.Abp.MassTransit.RabbitMQ
{
    [DependsOn(
        typeof(AbpMassTransitModule)
        )]
    public class AbpMassTransitRabbitMqModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {

        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {

            var massTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();

            var rabbitMqOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitRabbitMqOptions>();

            context.Services.AddMassTransit(x =>
            {
                foreach (var preConfigure in massTransitOptions.PreConfigures)
                {
                    preConfigure(x);
                }

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(rabbitMqOptions.Host, rabbitMqOptions.Port, rabbitMqOptions.VirtualHost, h =>
                    {
                        h.Username(rabbitMqOptions.Username);
                        h.Password(rabbitMqOptions.Password);
                        //SSL
                        if (rabbitMqOptions.UseSsl && rabbitMqOptions.ConfigureSsl != null)
                        {
                            h.UseSsl(rabbitMqOptions.ConfigureSsl);
                        }
                    });

                    cfg.ReceiveEndpoint("", e =>
                    {
                        e.Consumer<Class111Consumer>();
                    });

                    cfg.ConfigureEndpoints(ctx);
                });

            });
        }


    }
}

public class Class111
{

}

public class Class111Consumer : IConsumer
{

}
