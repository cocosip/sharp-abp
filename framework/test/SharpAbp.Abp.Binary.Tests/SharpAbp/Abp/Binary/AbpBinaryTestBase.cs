using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Binary
{
    public abstract class AbpBinaryTestBase : AbpIntegratedTest<AbpBinaryTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
