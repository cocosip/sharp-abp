using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public abstract class DbConnectionsManagementTestBase : AbpIntegratedTest<DbConnectionsManagementTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
