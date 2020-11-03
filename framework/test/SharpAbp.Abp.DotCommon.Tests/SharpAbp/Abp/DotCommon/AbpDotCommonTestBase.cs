using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.DotCommon
{
    public abstract class AbpDotCommonTestBase : AbpIntegratedTest<AbpDotCommonTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
