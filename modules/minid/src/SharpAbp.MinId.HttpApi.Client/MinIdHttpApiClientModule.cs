using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace SharpAbp.MinId
{
    [DependsOn(
        typeof(MinIdApplicationContractsModule),
        typeof(AbpHttpClientModule))]
    public class MinIdHttpApiClientModule : AbpModule
    {
        public const string RemoteServiceName = "MinId";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(MinIdApplicationContractsModule).Assembly,
                RemoteServiceName
            );
        }
    }
}
