using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Data.DbConnections
{
    public abstract class AbpDataDbConnectionsTestBase : AbpIntegratedTest<AbpDataDbConnectionsTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
