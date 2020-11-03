using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.AutoS3.KS3
{
    public abstract class AbpAutoS3KS3TestBase : AbpIntegratedTest<AbpAutoS3KS3TestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
