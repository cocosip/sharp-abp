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
