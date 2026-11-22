using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.Scopes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenIddict.Abstractions;
using Volo.Abp.OpenIddict.Applications;

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
            context.Services.Replace(ServiceDescriptor.Scoped<IOpenIddictApplicationStore<OpenIddictApplicationModel>, TransactionalApplicationStore>());
            context.Services.Replace(ServiceDescriptor.Scoped<IAbpOpenIdApplicationStore, TransactionalApplicationStore>());
            context.Services.Replace(ServiceDescriptor.Scoped<IOpenIddictScopeCache<OpenIddictScopeModel>, StoreBackedScopeCache>());
            context.Services.TryAddTransient<AbpOpenIddictScopeCache>();
        }
    }
}
