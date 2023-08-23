using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.SM
{
    public abstract class AbpSmTestBase : AbpIntegratedTest<AbpSmTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
