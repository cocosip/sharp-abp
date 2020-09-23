using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Consul
{

    public abstract class AbpConsulTestBase : AbpIntegratedTest<AbpConsulTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
