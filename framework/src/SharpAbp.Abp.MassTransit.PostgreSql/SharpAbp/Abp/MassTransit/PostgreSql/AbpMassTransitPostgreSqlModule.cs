using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;


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
                    options.DefaultPublishTopologyConfigurator = new Action<ISqlMessagePublishTopologyConfigurator, Type>((c, t) =>
                    {
                        //c.Exclude = true;
                    });

                    options.DefaultFilter = new Func<Type, bool>(t => true);
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
                    var builder = new NpgsqlConnectionStringBuilder(postgreSqlOptions.ConnectionString);
                    context.Services.AddOptions<SqlTransportOptions>().Configure(options =>
                    {
                        options.Host = postgreSqlOptions.SqlTransportOptions?.Host;
                        options.Database = postgreSqlOptions.SqlTransportOptions?.Database ?? "masstransit";
                        options.Schema = postgreSqlOptions.SqlTransportOptions?.Schema ?? "transport";
                        options.Role = postgreSqlOptions.SqlTransportOptions?.Role ?? "transport";
                        options.Username = postgreSqlOptions.SqlTransportOptions?.Username ?? "postgres";
                        options.Password = postgreSqlOptions.SqlTransportOptions?.Password ?? "postgres";
                        options.AdminUsername = postgreSqlOptions.SqlTransportOptions?.AdminUsername ?? builder.Username;
                        options.AdminPassword = postgreSqlOptions.SqlTransportOptions?.AdminPassword ?? builder.Password;

                        postgreSqlOptions.SqlTransportConfigure?.Invoke(options);
                    });

                    //PostgreSql数据自动迁移
                    context.Services.AddPostgresMigrationHostedService(postgreSqlOptions.Create, postgreSqlOptions.Delete);

                    context.Services.AddMassTransit(x =>
                    {
                        //Masstransit preConfigure
                        foreach (var preConfigure in abpMassTransitOptions.PreConfigures)
                        {
                            preConfigure(x);
                        }

                        x.SetKebabCaseEndpointNameFormatter();

                        //consumer
                        foreach (var consumer in postgreSqlOptions.Consumers)
                        {
                            consumer.Configure?.Invoke(x);

                            if (consumer.Types.Count > 0)
                            {
                                x.AddConsumers(consumer.Filter ?? postgreSqlOptions.DefaultFilter, consumer.Types.ToArray());
                            }
                        }

                        x.UsingPostgres((ctx, cfg) =>
                        {
                            //PostgreSql preConfigure
                            foreach (var preConfigure in postgreSqlOptions.PostgreSqlPreConfigures)
                            {
                                preConfigure(ctx, cfg);
                            }

                            cfg.UseDbMessageScheduler();
                            cfg.AutoDeleteOnIdle = postgreSqlOptions.AutoDeleteOnIdle;
                            cfg.AutoStart = postgreSqlOptions.AutoStart;
                            cfg.PrefetchCount = postgreSqlOptions.PrefetchCount;
                            cfg.ConcurrentMessageLimit = postgreSqlOptions.ConcurrentMessageLimit;
                            cfg.ConfigureEndpoints(ctx);

                            //PostgreSql configure
                            foreach (var configure in postgreSqlOptions.PostgreSqlConfigures)
                            {
                                configure(ctx, cfg);
                            }

                            //Producer
                            foreach (var producer in postgreSqlOptions.Producers)
                            {
                                var configure = producer.Configure ?? postgreSqlOptions.DefaultPublishTopologyConfigurator;
                                cfg.AddPublishMessageTypes(producer.MessageTypes, producer.Configure);
                            }

                            //PostgreSql postConfigure
                            foreach (var postConfigure in postgreSqlOptions.PostgreSqlPostConfigures)
                            {
                                postConfigure(ctx, cfg);
                            }

                        });

                    });

                    //Host
                    MassTransitSetupUtil.ConfigureMassTransitHost(context);

                }
            }
            return Task.CompletedTask;
        }

    }
}
