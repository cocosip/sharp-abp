using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.FreeRedis
{
    public abstract class AbpFreeRedisTestBase : AbpIntegratedTest<AbpFreeRedisTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
