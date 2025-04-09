using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.MassTransit.SqlServer
{
    [DependsOn(
        typeof(AbpMassTransitModule)
        )]
    public class AbpMassTransitSqlServerModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var abpMassTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();

            if (abpMassTransitOptions.Provider!.Equals(MassTransitSqlServerConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
            {
                PreConfigure<AbpMassTransitSqlServerOptions>(options => options.PreConfigure(configuration));

                var sqlServerOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitSqlServerOptions>();

                PreConfigure<AbpMassTransitSqlServerOptions>(options =>
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
                if (abpMassTransitOptions.Provider!.Equals(MassTransitSqlServerConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
                {
                    Configure<AbpMassTransitSqlServerOptions>(options =>
                    {
                        var actions = context.Services.GetPreConfigureActions<AbpMassTransitSqlServerOptions>();
                        foreach (var action in actions)
                        {
                            action(options);
                        }
                    });

                    var sqlServerOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitSqlServerOptions>()!;
                    var builder = new SqlConnectionStringBuilder(sqlServerOptions.ConnectionString);
                    context.Services.AddOptions<SqlTransportOptions>().Configure(options =>
                    {
                        options.Host = sqlServerOptions.SqlTransportOptions?.Host;
                        options.Database = sqlServerOptions.SqlTransportOptions?.Database ?? "masstransit";
                        options.Schema = sqlServerOptions.SqlTransportOptions?.Schema ?? "transport";
                        options.Role = sqlServerOptions.SqlTransportOptions?.Role ?? "transport";
                        options.Username = sqlServerOptions.SqlTransportOptions?.Username ?? "postgres";
                        options.Password = sqlServerOptions.SqlTransportOptions?.Password ?? "postgres";
                        options.AdminUsername = sqlServerOptions.SqlTransportOptions?.AdminUsername ?? builder.UserID;
                        options.AdminPassword = sqlServerOptions.SqlTransportOptions?.AdminPassword ?? builder.Password;

                        sqlServerOptions.SqlTransportConfigure?.Invoke(options);
                    });

                    //PostgreSql数据自动迁移
                    context.Services.AddSqlServerMigrationHostedService(sqlServerOptions.Create, sqlServerOptions.Delete);

                    context.Services.AddMassTransit(x =>
                    {
                        //Masstransit preConfigure
                        foreach (var preConfigure in abpMassTransitOptions.PreConfigures)
                        {
                            preConfigure(x);
                        }

                        x.SetKebabCaseEndpointNameFormatter();

                        //consumer
                        foreach (var consumer in sqlServerOptions.Consumers ?? [])
                        {
                            consumer.Configure?.Invoke(x);

                            if (consumer.Types.Count > 0)
                            {
                                x.AddConsumers(consumer.Filter ?? sqlServerOptions.DefaultFilter, consumer.Types.ToArray());
                            }
                        }

                        x.UsingSqlServer((ctx, cfg) =>
                        {
                            //PostgreSql preConfigure
                            foreach (var preConfigure in sqlServerOptions.SqlServerPreConfigures ?? [])
                            {
                                preConfigure(ctx, cfg);
                            }

                            cfg.UseSqlMessageScheduler();
                            cfg.AutoDeleteOnIdle = sqlServerOptions.AutoDeleteOnIdle;
                            cfg.AutoStart = sqlServerOptions.AutoStart;
                            cfg.PrefetchCount = sqlServerOptions.PrefetchCount;
                            cfg.ConcurrentMessageLimit = sqlServerOptions.ConcurrentMessageLimit;
                            cfg.ConfigureEndpoints(ctx);

                            //PostgreSql configure
                            foreach (var configure in sqlServerOptions.SqlServerConfigures ?? [])
                            {
                                configure(ctx, cfg);
                            }

                            //Producer
                            foreach (var producer in sqlServerOptions.Producers ?? [])
                            {
                                var configure = producer.Configure ?? sqlServerOptions.DefaultPublishTopologyConfigurator;
                                cfg.AddPublishMessageTypes(producer.MessageTypes, producer.Configure);
                            }

                            //PostgreSql postConfigure
                            foreach (var postConfigure in sqlServerOptions.SqlServerPostConfigures ?? [])
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
