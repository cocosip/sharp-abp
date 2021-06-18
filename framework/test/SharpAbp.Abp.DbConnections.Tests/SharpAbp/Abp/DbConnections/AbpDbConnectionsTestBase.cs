using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.DbConnections
{
    public abstract class AbpDbConnectionsTestBase : AbpIntegratedTest<AbpDbConnectionsTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
