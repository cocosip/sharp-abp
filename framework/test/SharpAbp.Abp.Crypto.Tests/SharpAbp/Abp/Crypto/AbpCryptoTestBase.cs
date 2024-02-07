using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Crypto
{
    public abstract class AbpCryptoTestBase : AbpIntegratedTest<AbpCryptoTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
