using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using MassTransit;


namespace SharpAbp.Abp.MassTransit.PostgreSql
{
    [DependsOn(
        typeof(AbpMassTransitModule)
        )]
    public class AbpMassTransitPostgreSqlModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var abpMassTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();

            if (abpMassTransitOptions.Provider.Equals(MassTransitPostgreSqlConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
            {
                PreConfigure<AbpMassTransitPostgreSqlOptions>(options => options.PreConfigure(configuration));

                var postgreSqlOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitPostgreSqlOptions>();

                PreConfigure<AbpMassTransitPostgreSqlOptions>(options =>
                {
                    //options.DefaultExchangeNameFormatFunc = RabbitMqUtil.ExchangeNameFormat;
                    //options.DefaultQueueNameFormatFunc = RabbitMqUtil.QueueNameFormat;

                    //options.DefaultPublishTopologyConfigure = new Action<IRabbitMqMessagePublishTopologyConfigurator>(c =>
                    //{
                    //    c.AutoDelete = rabbitMqOptions.DefaultAutoDelete;
                    //    c.Durable = rabbitMqOptions.DefaultDurable;
                    //    c.ExchangeType = rabbitMqOptions.DefaultExchangeType;
                    //});

                    //options.DefaultReceiveEndpointConfigure = new Action<string, string, IRabbitMqReceiveEndpointConfigurator>((exchangeName, queueName, c) =>
                    //{
                    //    c.ConcurrentMessageLimit = rabbitMqOptions.DefaultConcurrentMessageLimit;
                    //    c.PrefetchCount = rabbitMqOptions.DefaultPrefetchCount;
                    //    c.AutoDelete = rabbitMqOptions.DefaultAutoDelete;
                    //    c.Durable = rabbitMqOptions.DefaultDurable;
                    //    c.ExchangeType = rabbitMqOptions.DefaultExchangeType;
                    //});
                });
            }

            return Task.CompletedTask;
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PostConfigureServicesAsync(context));
        }

        public override Task PostConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var abpMassTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();
            if (abpMassTransitOptions != null)
            {
                if (abpMassTransitOptions.Provider.Equals(MassTransitPostgreSqlConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
                {
                    Configure<AbpMassTransitPostgreSqlOptions>(options =>
                    {
                        var actions = context.Services.GetPreConfigureActions<AbpMassTransitPostgreSqlOptions>();
                        foreach (var action in actions)
                        {
                            action(options);
                        }
                    });

                    var postgreSqlOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitPostgreSqlOptions>();

                    context.Services.AddMassTransit(x =>
                    {
                        //Masstransit preConfigure
                        foreach (var preConfigure in abpMassTransitOptions.PreConfigures)
                        {
                            preConfigure(x);
                        }


                    });

                    //Host
                    MassTransitSetupUtil.ConfigureMassTransitHost(context);
                }
            }
            return Task.CompletedTask;
        }

    }
}
