using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore
{
    public abstract class FileStoringManagementEntityFrameworkCoreTestBase : AbpIntegratedTest<FileStoringManagementEntityFrameworkCoreTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
