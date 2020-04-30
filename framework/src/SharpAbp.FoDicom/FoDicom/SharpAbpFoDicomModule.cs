using FellowOakDicom;
using FellowOakDicom.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.FoDicom.Logging;
using System.IO;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.FoDicom
{
    public class SharpAbpFoDicomModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services
                .Configure<FoDicomOption>(x => { x.TemporaryFilePath = Path.GetTempPath(); })
                .AddFellowOakDicom()
                .AddLogManager<MicrosoftLogManager>()
                .Replace<IServiceProviderHost, CustomServiceProviderHost>(ServiceLifetime.Singleton)
                //.AddSingleton<IServiceProviderHost, CustomServiceProviderHost>()
            ;
        }


        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var option = context.ServiceProvider.GetService<IOptions<FoDicomOption>>().Value;
            TemporaryFile.StoragePath = option.TemporaryFilePath;

            //var host = context.ServiceProvider.GetService<IServiceProviderHost>();
            //((CustomServiceProviderHost)host).SetupDI(context.ServiceProvider);
        }

    }
}
