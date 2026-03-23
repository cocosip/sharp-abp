using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Faster
{
    public abstract class AbpFasterTestBase : AbpIntegratedTest<AbpFasterTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
