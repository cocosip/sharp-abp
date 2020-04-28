using DotCommon.DependencyInjection;
using DotCommon.Json4Net;
using DotCommon.Serializing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Modularity;

namespace SharpAbp.DotCommon.Json4Net
{

    [DependsOn(typeof(SharpAbpDotCommonModule))]
    public class SharpAbpDotCommonJson4NetModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Replace(ServiceDescriptor.Singleton<IJsonSerializer, NewtonsoftJsonSerializer>());
        }
    }
}
