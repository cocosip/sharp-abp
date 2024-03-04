using SharpAbp.Abp.CryptoVault;
using System.Threading.Tasks;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AbpCachingModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpTransformSecurityManagementDomainSharedModule),
        typeof(AbpCryptoVaultDomainModule)
        )]
    public class AbpTransformSecurityManagementDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }


        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
           
 
            return Task.CompletedTask;
        }

    }
}
