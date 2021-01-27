using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Core
{
    public abstract class SharpAbpCoreTestBase : AbpIntegratedTest<SharpAbpCoreTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
