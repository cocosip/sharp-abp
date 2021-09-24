using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Identity
{
    public abstract class IdentityApplicationTestBase : AbpIntegratedTest<IdentityApplicationTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
