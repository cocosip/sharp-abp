using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.CryptoVault
{
    public abstract class AbpCryptoVaultApplicationTestBase : AbpIntegratedTest<AbpCryptoVaultApplicationTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
