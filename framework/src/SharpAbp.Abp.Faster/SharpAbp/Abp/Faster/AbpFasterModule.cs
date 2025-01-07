using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.IO;
using Volo.Abp.Modularity;
using Volo.Abp.Serialization;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Faster
{
    [DependsOn(
        typeof(AbpSerializationModule),
        typeof(AbpThreadingModule)
        )]
    public class AbpFasterModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddTransient(
                typeof(IFasterLogger<>),
                typeof(FasterLogger<>)
            );

            context.Services.AddTransient(
                typeof(IFasterLogger),
                serviceProvider => serviceProvider
                    .GetRequiredService<IFasterLogger<DefaultFasterLog>>()
            );

            Configure<AbpFasterOptions>(options =>
            {
                options.RootPath = Path.Combine(AppContext.BaseDirectory, "faster-logs");
            });

            return Task.CompletedTask;
        }



        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            AsyncHelper.RunSync(() => OnApplicationInitializationAsync(context));
        }


        public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            var options = context.ServiceProvider.GetService<IOptions<AbpFasterOptions>>().Value;
            DirectoryHelper.CreateIfNotExists(options.RootPath);
            return base.OnApplicationInitializationAsync(context);
        }
    }
}
