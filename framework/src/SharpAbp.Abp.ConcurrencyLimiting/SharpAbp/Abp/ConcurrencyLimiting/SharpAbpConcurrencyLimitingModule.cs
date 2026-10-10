using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.ConcurrencyLimiting.Localization;
using Volo.Abp;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Users;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    [DependsOn(typeof(AbpUsersAbstractionModule))]
    public class SharpAbpConcurrencyLimitingModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.OnRegistered(ConcurrencyLimitingInterceptorRegistrar.RegisterIfNeeded);
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddOptions<SharpAbpConcurrencyLimitingOptions>();

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<SharpAbpConcurrencyLimitingModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<ConcurrencyLimitingResource>("en")
                    .AddVirtualJson("/SharpAbp/Abp/ConcurrencyLimiting/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("ConcurrencyLimiting", typeof(ConcurrencyLimitingResource));
            });

            return Task.CompletedTask;
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            // Materialize and validate immutable startup snapshots before serving operations.
            context.ServiceProvider.GetRequiredService<IConcurrencyLimitingPolicyProvider>();
            context.ServiceProvider.GetRequiredService<IConcurrencyLimitingConfiguration>();
        }
    }
}
