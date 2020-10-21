using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public abstract class AbpMicroLoadBalancerOptionsTestBase : AbpIntegratedTest<AbpMicroLoadBalancerOptionsTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
