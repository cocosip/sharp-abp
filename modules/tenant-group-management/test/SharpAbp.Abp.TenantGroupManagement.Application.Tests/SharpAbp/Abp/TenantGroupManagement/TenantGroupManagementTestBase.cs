using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public abstract class TenantGroupManagementTestBase : AbpIntegratedTest<TenantGroupManagementTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
