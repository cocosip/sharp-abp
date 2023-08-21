using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.SM
{
    public class AbpSmModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpSm2EncryptionOptions>(options =>
            {
                options.DefaultCurve = Sm2EncryptionNames.CurveSm2p256v1;
            });

            Configure<AbpSm4EncryptionOptions>(options =>
            {
                options.DefaultIV = Encoding.UTF8.GetBytes("1234567890abcdef");
                options.DefaultMode = Sm4EncryptionNames.ModeECB;
                options.DefaultPadding = Sm4EncryptionNames.NoPadding;
            });

            return Task.CompletedTask;
        }

    }
}
