using System;
using System.Threading.Tasks;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Modularity;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.OperationRateLimiting.RealInfrastructure
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            if (args.Length != 3 || args[0] != "--operation-worker")
            {
                return 0;
            }
            try
            {
                using (var application = AbpApplicationFactory.Create<RedisAcceptanceModule>(options => options.UseAutofac()))
                {
                    await application.InitializeAsync();
                    var checker = application.ServiceProvider.GetRequiredService<IOperationRateLimitingChecker>();
                    if (args[1] == "reset")
                    {
                        await checker.ResetAsync("acceptance", args[2]);
                        Console.WriteLine("RESULT:reset");
                    }
                    else
                    {
                        var connection = application.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
                        var database = connection.GetDatabase();
                        var prefix = Environment.GetEnvironmentVariable("SHARP_ABP_OPERATION_ACCEPTANCE_NAMESPACE")!;
                        await database.StringIncrementAsync(prefix + ":ready");
                        var deadline = DateTime.UtcNow.AddSeconds(30);
                        while (await database.StringGetAsync(prefix + ":gate") != "go")
                        {
                            if (DateTime.UtcNow >= deadline)
                            {
                                throw new TimeoutException("Acceptance start gate timed out.");
                            }
                            await Task.Delay(25);
                        }
                        var admitted = 0;
                        for (var index = 0; index < 100; index++)
                        {
                            try
                            {
                                await checker.CheckAsync("acceptance", args[2]);
                                admitted++;
                            }
                            catch (OperationRateLimitExceededException)
                            {
                            }
                        }
                        Console.WriteLine("RESULT:" + admitted);
                    }
                    await application.ShutdownAsync();
                    return 0;
                }
            }
            catch (Exception exception)
            {
                // Connection strings can contain credentials: record only a type, never exception text.
                Console.WriteLine("INFRASTRUCTURE_FAILURE:" + exception.GetType().Name);
                return 2;
            }
        }
    }

    [DependsOn(typeof(SharpAbpOperationRateLimitingModule), typeof(AbpDistributedLockingModule), typeof(AbpAutofacModule))]
    public class RedisAcceptanceModule : AbpModule
    {
        public RedisAcceptanceModule()
        {
            SkipAutoServiceRegistration = true;
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = Environment.GetEnvironmentVariable("SHARP_ABP_OPERATION_REDIS");
            var applicationName = Environment.GetEnvironmentVariable("SHARP_ABP_OPERATION_ACCEPTANCE_NAMESPACE");
            if (string.IsNullOrWhiteSpace(configuration) || string.IsNullOrWhiteSpace(applicationName))
            {
                throw new InvalidOperationException("Redis acceptance configuration is missing.");
            }
            var connection = ConnectionMultiplexer.Connect(configuration);
            context.Services.AddSingleton<IConnectionMultiplexer>(connection);
            context.Services.AddSingleton<IDistributedLockProvider>(new RedisDistributedSynchronizationProvider(connection.GetDatabase()));
            context.Services.AddStackExchangeRedisCache(options => options.Configuration = configuration);
            Configure<AbpClockOptions>(options => options.Kind = DateTimeKind.Utc);
            Configure<SharpAbpOperationRateLimitingOptions>(options =>
            {
                options.ApplicationName = applicationName;
                options.CoordinationScope = OperationRateLimitingCoordinationScope.Distributed;
                options.AddPolicy("acceptance", builder => builder.WithFixedWindow(TimeSpan.FromMinutes(10), 7).PartitionByParameter());
            });
        }
    }
}
