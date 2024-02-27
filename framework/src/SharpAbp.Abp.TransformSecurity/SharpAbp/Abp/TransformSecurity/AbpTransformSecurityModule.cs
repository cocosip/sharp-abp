using SharpAbp.Abp.Crypto;
using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.Crypto.SM2;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TransformSecurity
{
    [DependsOn(
        typeof(AbpCryptoModule),
        typeof(AbpObjectExtendingModule)
        )]
    public class AbpTransformSecurityModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpTransformSecurityRSAOptions>(options =>
            {
                options.Padding = RSAPaddingNames.PKCS1Padding;
                options.KeySize = 2048;
            });

            Configure<AbpTransformSecuritySM2Options>(options =>
            {
                options.Curve = Sm2EncryptionNames.CurveSm2p256v1;
                options.Mode = Org.BouncyCastle.Crypto.Engines.SM2Engine.Mode.C1C2C3;
            });

            Configure<AbpTransformSecurityOptions>(options =>
            {
                options.Enabled = false;
                options.EncryptionAlgo = "RSA";
            });

            return Task.CompletedTask;
        }

    }
}
