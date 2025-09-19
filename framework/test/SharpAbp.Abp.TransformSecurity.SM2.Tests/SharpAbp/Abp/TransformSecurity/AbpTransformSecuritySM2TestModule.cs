using SharpAbp.Abp.Crypto.SM2;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TransformSecurity
{
    [DependsOn(
     typeof(AbpTransformSecurityModule),
     typeof(AbpTestBaseModule),
     typeof(AbpAutofacModule)
     )]
    public class AbpTransformSecuritySM2TestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpTransformSecurityOptions>(options =>
            {
                options.IsEnabled = true;
                options.EncryptionAlgo = "SM2";
                options.BizTypes.Add("UpdatePassword");
            });

            Configure<AbpTransformSecuritySM2Options>(options =>
            {
                options.Curve = Sm2EncryptionNames.CurveSm2p256v1;
                options.Mode = Org.BouncyCastle.Crypto.Engines.SM2Engine.Mode.C1C3C2;
            });
            return Task.CompletedTask;
        }
    }
}
