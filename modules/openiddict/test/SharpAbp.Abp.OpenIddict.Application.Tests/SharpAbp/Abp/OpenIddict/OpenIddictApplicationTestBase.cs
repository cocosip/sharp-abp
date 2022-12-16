using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.OpenIddict
{
    public abstract class OpenIddictApplicationTestBase : AbpIntegratedTest<OpenIddictApplicationTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
