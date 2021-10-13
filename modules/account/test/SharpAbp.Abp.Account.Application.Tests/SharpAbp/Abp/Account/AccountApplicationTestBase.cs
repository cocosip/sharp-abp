using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Account
{
    public class AccountApplicationTestBase : AbpIntegratedTest<AccountApplicationTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
