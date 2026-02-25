using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Validation;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FileStoring
{
    [DependsOn(
        typeof(SharpAbpValidationModule)
        )]
    public class AbpFileStoringAbstractionsModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpFileStoringAbstractionsOptions>(options => { });

            return Task.CompletedTask;
        }



        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PostConfigureServicesAsync(context));
        }

        public override Task PostConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var actions = context.Services.GetPreConfigureActions<AbpFileStoringAbstractionsOptions>();
            foreach (var action in actions)
            {
                Configure(action);
            }
            return Task.CompletedTask;
        }


    }
}
