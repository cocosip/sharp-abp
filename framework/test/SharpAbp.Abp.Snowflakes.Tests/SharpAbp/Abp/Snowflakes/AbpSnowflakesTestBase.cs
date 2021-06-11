using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Snowflakes
{
    public abstract class AbpSnowflakesTestBase : AbpIntegratedTest<AbpSnowflakesTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
