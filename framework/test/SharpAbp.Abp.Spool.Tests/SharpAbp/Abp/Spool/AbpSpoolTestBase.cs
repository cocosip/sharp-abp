using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Spool
{
    public abstract class AbpSpoolTestBase : AbpIntegratedTest<AbpSpoolTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
