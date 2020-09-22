using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.CSRedisCore
{
    public abstract class AbpCSRedisCoreTestBase : AbpIntegratedTest<AbpCSRedisCoreTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
