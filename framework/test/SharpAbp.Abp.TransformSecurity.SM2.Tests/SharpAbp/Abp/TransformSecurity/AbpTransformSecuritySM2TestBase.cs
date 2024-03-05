using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.TransformSecurity
{
    public abstract class AbpTransformSecuritySM2TestBase : AbpIntegratedTest<AbpTransformSecuritySM2TestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
