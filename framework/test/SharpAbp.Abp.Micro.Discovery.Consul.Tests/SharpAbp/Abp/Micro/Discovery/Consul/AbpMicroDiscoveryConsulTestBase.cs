using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public abstract class AbpMicroDiscoveryConsulTestBase : AbpIntegratedTest<AbpMicroDiscoveryConsulTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
