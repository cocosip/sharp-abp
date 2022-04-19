using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.FileStoring
{
    public abstract class AbpFileStoringAllTestBase : AbpIntegratedTest<AbpFileStoringAllTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }

        public override void Dispose()
        {

        }
    }
}
