using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public abstract class AbpEntityFrameworkCoreTestBase : AbpIntegratedTest<AbpEntityFrameworkCoreTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}