using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
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
            var actions = context.Services.GetPreConfigureActions<AbpFileStoringAbstractionsOptions>();
            foreach (var action in actions)
            {
                Configure(action);
            }
        }
    }
}
