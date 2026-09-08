using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.EntityFrameworkCore;
using Volo.Abp.Dapper;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Dapper
{
    [DependsOn(
        typeof(SharpAbpEntityFrameworkCoreModule),
        typeof(AbpDapperModule)
        )]
    public class SharpAbpDapperModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            PreConfigure<SharpAbpDapperOptions>(options =>
            {
                options.TypeMappings.Add(new ExtraPropertyDictionaryTypeMapping());
            });

            return Task.CompletedTask;
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PostConfigureServicesAsync(context));
        }

        public override Task PostConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var options = new SharpAbpDapperOptions();
            foreach (var configureAction in context.Services.GetPreConfigureActions<SharpAbpDapperOptions>())
            {
                configureAction(options);
            }

            DapperTypeHandlerRegistrar.Register(options);

            return Task.CompletedTask;
        }

    }
}
