using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.TransformSecurity
{
    public abstract class AbpTransformSecurityTestBase : AbpIntegratedTest<AbpTransformSecurityTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
