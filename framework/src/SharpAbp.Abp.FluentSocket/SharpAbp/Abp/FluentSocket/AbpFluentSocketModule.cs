using FluentSocket;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FluentSocket
{
    public class AbpFluentSocketModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddFluentSocket();
        }
    }
}
