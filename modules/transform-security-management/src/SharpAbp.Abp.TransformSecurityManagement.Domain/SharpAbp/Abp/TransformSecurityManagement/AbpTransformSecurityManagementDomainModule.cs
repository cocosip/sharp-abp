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
        typeof(AbpTransformSecurityManagementDomainSharedModule)
        )]
    public class AbpTransformSecurityManagementDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }


        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            //Configure<AbpCryptoVaultOptions>(options =>
            //{
            //    options.RSACount = 10;
            //    options.RSAKeySize = 2048;
            //    options.SM2Count = 10;
            //    options.SM2Curve = Sm2EncryptionNames.CurveSm2p256v1;
            //});
 
            return Task.CompletedTask;
        }

    }
}
