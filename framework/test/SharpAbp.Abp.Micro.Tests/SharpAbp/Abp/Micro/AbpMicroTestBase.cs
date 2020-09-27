using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Micro
{
    public abstract class AbpMicroTestBase : AbpIntegratedTest<AbpMicroTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
