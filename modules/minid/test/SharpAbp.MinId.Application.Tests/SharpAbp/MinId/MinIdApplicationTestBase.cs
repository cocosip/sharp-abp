using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.MinId
{
    public abstract class MinIdApplicationTestBase : AbpIntegratedTest<MinIdApplicationTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }


   


        public override void Dispose()
        {

        }
    }
}
