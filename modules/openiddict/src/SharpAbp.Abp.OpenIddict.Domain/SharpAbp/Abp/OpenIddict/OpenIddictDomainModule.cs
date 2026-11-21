using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.Scopes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenIddict.Abstractions;

namespace SharpAbp.Abp.OpenIddict
{
    [DependsOn(
        typeof(AbpOpenIddictDomainModule),
        typeof(OpenIddictDomainSharedModule)
        )]
    public class OpenIddictDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Replace(ServiceDescriptor.Scoped<IOpenIddictScopeCache<OpenIddictScopeModel>, StoreBackedScopeCache>());
            context.Services.TryAddTransient<AbpOpenIddictScopeCache>();
        }
    }
}
