using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.AuditLogging
{
    public abstract class AuditLoggingApplicationTestBase : AbpIntegratedTest<AuditLoggingApplicationTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
