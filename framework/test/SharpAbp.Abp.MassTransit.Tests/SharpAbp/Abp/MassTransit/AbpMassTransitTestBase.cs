using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.MassTransit
{
    public abstract class AbpMassTransitTestBase : AbpIntegratedTest<AbpMassTransitTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
