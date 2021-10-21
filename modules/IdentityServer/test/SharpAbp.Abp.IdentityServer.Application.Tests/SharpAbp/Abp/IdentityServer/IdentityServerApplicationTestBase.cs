using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.IdentityServer
{
    public class IdentityServerApplicationTestBase : AbpIntegratedTest<IdentityServerApplicationTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
