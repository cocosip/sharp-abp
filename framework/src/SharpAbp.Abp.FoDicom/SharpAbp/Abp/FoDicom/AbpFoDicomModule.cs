using FellowOakDicom;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.FoDicom.Log;
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

    }
}
