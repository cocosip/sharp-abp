using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.CAP
{

    /// <summary>
    /// Cap module PostConfigure<CapOptions>  only support baseic propery configure
    /// </summary>
    public class AbpCapModule : AbpModule
    {
        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            var configure = context.Services.GetSingletonInstance<IConfigureOptions<CapOptions>>();

            context.Services.AddCap(configure.Configure);
        }
    }
}
