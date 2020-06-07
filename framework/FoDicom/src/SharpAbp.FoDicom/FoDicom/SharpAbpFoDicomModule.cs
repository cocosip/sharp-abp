using Dicom.IO;
using Dicom.Log;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpAbp.FoDicom.Logging;
using System;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.FoDicom
{
    public class SharpAbpFoDicomModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            context.Services.Configure<FoDicomOption>(configuration.GetSection(""));
        }


        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            //设置日志
            var loggerFactory = context.ServiceProvider.GetService<ILoggerFactory>();
            LogManager.SetImplementation(new MicrosoftLogManager(loggerFactory));

            //设置临时存储目录
            var option = context.ServiceProvider.GetService<IOptions<FoDicomOption>>().Value;
            if (!option.TemporaryFilePath.IsNullOrWhiteSpace())
            {
                TemporaryFile.StoragePath = option.TemporaryFilePath;
            }

        }
    }
}
