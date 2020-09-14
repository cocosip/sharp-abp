using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.FileStoring
{
    public abstract class AbpFileStoringTestBase : AbpIntegratedTest<AbpFileStoringTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
