using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Micro.Discovery
{
    public abstract class AbpMicroDiscoveryTestBase : AbpIntegratedTest<AbpMicroDiscoveryTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
