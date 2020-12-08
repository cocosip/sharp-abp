using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring
{
    [DependsOn(
        typeof(AbpMultiTenancyModule),
        typeof(AbpThreadingModule),
        typeof(AbpValidationModule)
        )]
    public class AbpFileStoringModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient(
                typeof(IFileContainer<>),
                typeof(FileContainer<>)
            );

            context.Services.AddTransient(
                typeof(IFileContainer),
                serviceProvider => serviceProvider
                    .GetRequiredService<IFileContainer<DefaultContainer>>()
            );
        }

    }
}