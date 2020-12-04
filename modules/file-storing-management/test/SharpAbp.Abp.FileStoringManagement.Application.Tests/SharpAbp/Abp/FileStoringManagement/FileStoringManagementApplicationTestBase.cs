using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.FileStoringManagement
{
    public abstract class FileStoringManagementApplicationTestBase : AbpIntegratedTest<FileStoringManagementApplicationTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
