using FellowOakDicom;
using FellowOakDicom.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SharpAbp.DotCommon.Json4Net;
using SharpAbp.FoDicom.Json;
using SharpAbp.FoDicom.Logging;
using System;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.FoDicom
{
    [DependsOn(typeof(SharpAbpDotCommonJson4NetModule))]
    public class SharpAbpFoDicomModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services
                .Configure<FoDicomOption>(c => { })
                .AddFellowOakDicom()
                .AddLogManager<MicrosoftLogManager>()
                .AddSingleton<IServiceProviderHost, FoDicomServiceProviderHost>();

            context.Services.Configure<JsonSerializerSettings>(c =>
            {
                c.Converters.Add(new JsonDicomConverter());
            });
        }


        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var option = context.ServiceProvider.GetService<IOptions<FoDicomOption>>().Value;
            //设置临时存储路径
            if (!option.TemporaryFilePath.IsNullOrWhiteSpace())
            {
                TemporaryFile.StoragePath = option.TemporaryFilePath;
            }

            var host = context.ServiceProvider.GetService<IServiceProviderHost>();
            ((FoDicomServiceProviderHost)host).SetupDI(context.ServiceProvider);

        }
    }
}
