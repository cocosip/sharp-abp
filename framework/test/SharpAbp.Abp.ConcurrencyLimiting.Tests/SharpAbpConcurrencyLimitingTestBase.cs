using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public abstract class SharpAbpConcurrencyLimitingTestBase : AbpIntegratedTest<SharpAbpConcurrencyLimitingTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }

}
