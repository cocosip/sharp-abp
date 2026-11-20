using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Binary.Protobuf
{
    [DependsOn(
        typeof(AbpBinaryAbstractionsModule)
        )]
    public class AbpBinaryProtobufModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpProtobufSerializerOptions>(options => { });
            return Task.CompletedTask;
        }

    }
}
