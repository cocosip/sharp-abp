using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public abstract class MapTenancyManagementApplicationTestBase : AbpIntegratedTest<MapTenancyManagementApplicationTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
