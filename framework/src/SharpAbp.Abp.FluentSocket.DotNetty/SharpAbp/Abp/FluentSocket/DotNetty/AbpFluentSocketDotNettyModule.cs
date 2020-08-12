using FluentSocket.DotNetty;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FluentSocket.DotNetty
{
    [DependsOn(typeof(AbpFluentSocketModule))]
    public class AbpFluentSocketDotNettyModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddFluentSocketDotNetty();
        }
    }
}
