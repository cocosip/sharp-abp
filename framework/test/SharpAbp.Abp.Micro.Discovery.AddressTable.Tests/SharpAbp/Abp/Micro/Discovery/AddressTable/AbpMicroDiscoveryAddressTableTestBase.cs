using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public abstract class AbpMicroDiscoveryAddressTableTestBase : AbpIntegratedTest<AbpMicroDiscoveryAddressTableTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
