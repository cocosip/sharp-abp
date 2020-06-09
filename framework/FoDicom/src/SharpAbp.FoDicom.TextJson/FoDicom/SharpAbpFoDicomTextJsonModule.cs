using Microsoft.Extensions.DependencyInjection;
using SharpAbp.FoDicom.Json;
using System.Text.Json;
using Volo.Abp.Modularity;

namespace SharpAbp.FoDicom
{
    [DependsOn(typeof(SharpAbpFoDicomModule))]
    public class SharpAbpFoDicomTextJsonModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.PreConfigure<JsonSerializerOptions>(c =>
            {
                c.Converters.Add(new TextJsonDicomConverter());
            });
        }
    }
}
