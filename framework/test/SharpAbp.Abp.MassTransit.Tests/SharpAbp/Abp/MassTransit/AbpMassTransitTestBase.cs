using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// Base class for MassTransit unit tests providing common test infrastructure
    /// </summary>
    public abstract class AbpMassTransitTestBase : AbpIntegratedTest<AbpMassTransitTestModule>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbpMassTransitTestBase"/> class
        /// </summary>
        protected AbpMassTransitTestBase()
        {
            
        }

        /// <summary>
        /// Sets the test output helper for logging during tests
        /// </summary>
        /// <param name="testOutputHelper">The test output helper instance</param>
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}