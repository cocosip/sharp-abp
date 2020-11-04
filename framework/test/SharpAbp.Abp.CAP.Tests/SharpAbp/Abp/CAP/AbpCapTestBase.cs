using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.CAP
{
    public abstract class AbpCapTestBase : AbpIntegratedTest<AbpCapTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    

    }
}
