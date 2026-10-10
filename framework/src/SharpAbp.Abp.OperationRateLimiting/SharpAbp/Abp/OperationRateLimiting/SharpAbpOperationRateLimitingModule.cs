using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.OperationRateLimiting.Localization;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Timing;
using Volo.Abp.Users;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.OperationRateLimiting
{
    [DependsOn(typeof(AbpCachingModule), typeof(AbpDistributedLockingAbstractionsModule), typeof(AbpTimingModule),
        typeof(AbpUsersAbstractionModule), typeof(AbpMultiTenancyAbstractionsModule), typeof(AbpLocalizationModule),
        typeof(AbpVirtualFileSystemModule))]
    public class SharpAbpOperationRateLimitingModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.OnRegistered(OperationRateLimitingInterceptorRegistrar.RegisterIfNeeded);
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<SharpAbpOperationRateLimitingModule>());
            Configure<AbpLocalizationOptions>(options => options.Resources.Add<OperationRateLimitingResource>("en")
                .AddVirtualJson("/SharpAbp/Abp/OperationRateLimiting/Localization/Resources"));
            Configure<AbpExceptionLocalizationOptions>(options => options.MapCodeNamespace(
                "OperationRateLimiting", typeof(OperationRateLimitingResource)));
            return Task.CompletedTask;
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var configuration = context.ServiceProvider.GetRequiredService<OperationRateLimitingConfiguration>();
            using (var scope = context.ServiceProvider.CreateScope())
            {
                var store = scope.ServiceProvider.GetRequiredService<IOperationRateLimitingStore>();
                if (store is DistributedCacheOperationRateLimitingStore &&
                    configuration.CoordinationScope == OperationRateLimitingCoordinationScope.Distributed &&
                    scope.ServiceProvider.GetRequiredService<IAbpDistributedLock>() is LocalAbpDistributedLock)
                {
                    throw new OperationRateLimitingConfigurationException("Distributed counters require a shared lock provider; LocalAbpDistributedLock is local.");
                }
                foreach (var policy in configuration.Policies.Values)
                {
                    foreach (var rule in policy.Rules)
                    {
                        if (!(scope.ServiceProvider.GetService(rule.RuleType) is IOperationRateLimitingRule))
                        {
                            throw new OperationRateLimitingConfigurationException("Rule is not registered: " + rule.RuleType.FullName);
                        }
                    }
                }
            }
        }
    }
}
