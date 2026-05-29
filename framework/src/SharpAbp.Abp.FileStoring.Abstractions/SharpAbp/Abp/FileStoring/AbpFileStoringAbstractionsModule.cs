using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
            context.Services.AddSingleton<IFilePathContextAccessor>(
                AsyncLocalFilePathContextAccessor.Instance
            );
            context.Services.AddTransient<IFilePathContextResolver, DefaultFilePathContextResolver>();

            var configuration = context.Services.GetConfiguration();
            Configure<AbpFileStoringAbstractionsOptions>(options =>
            {
                var filePathBuilderEntry = configuration
                    .GetSection("FileStoringOptions:FilePathBuilder")
                    .Get<FilePathBuilderEntry>();
                filePathBuilderEntry?.ApplyTo(options);
            });
            Configure<AbpFilePathContextResolveOptions>(options => { });

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
