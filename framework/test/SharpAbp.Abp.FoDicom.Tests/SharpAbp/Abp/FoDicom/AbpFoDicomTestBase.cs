using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.FoDicom
{
    public abstract class AbpFoDicomTestBase : AbpIntegratedTest<AbpFoDicomTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
