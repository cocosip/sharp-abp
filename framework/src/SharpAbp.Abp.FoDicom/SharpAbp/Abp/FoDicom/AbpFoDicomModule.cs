using FellowOakDicom;
using FellowOakDicom.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.FoDicom.Log;
using System;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FoDicom
{
    public class AbpFoDicomModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services
                .Configure<FoDicomOption>(c => { })
                .AddFellowOakDicom()
                .AddLogManager<MicrosoftLogManager>();
        }


        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var option = context.ServiceProvider.GetRequiredService<IOptions<FoDicomOption>>().Value;
            if (!option.TemporaryFilePath.IsNullOrWhiteSpace())
            {
                TemporaryFile.StoragePath = option.TemporaryFilePath;
            }
        }

    }
}
