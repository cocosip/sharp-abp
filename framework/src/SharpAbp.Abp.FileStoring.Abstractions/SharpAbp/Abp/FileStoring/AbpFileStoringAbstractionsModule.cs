using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring
{
    [DependsOn(
        typeof(AbpValidationModule)
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
