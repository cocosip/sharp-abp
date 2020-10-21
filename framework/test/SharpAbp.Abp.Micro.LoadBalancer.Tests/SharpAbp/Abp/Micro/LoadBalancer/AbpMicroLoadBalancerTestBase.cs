using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public abstract class AbpMicroLoadBalancerTestBase : AbpIntegratedTest<AbpMicroLoadBalancerTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
