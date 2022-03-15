using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.Account
{
    [DependsOn(
        typeof(AccountApplicationContractsModule),
        typeof(AbpHttpClientModule)
        )]
    public class AccountHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddStaticHttpClientProxies(
                typeof(AccountApplicationContractsModule).Assembly,
                AccountRemoteServiceConsts.RemoteServiceName
            );

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AccountHttpApiClientModule>();
            });
        }
    }
}
