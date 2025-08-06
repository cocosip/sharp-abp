using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Xunit;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// Unit tests for <see cref="AbpMassTransitModule"/>
    /// </summary>
    public class AbpMassTransitModuleTest : AbpIntegratedTest<AbpMassTransitBasicTestModule>
    {
        /// <summary>
        /// Tests that the module is loaded correctly and services are registered
        /// </summary>
        [Fact]
        public void Module_Should_Be_Loaded_Correctly()
        {
            // Act & Assert
            var module = GetRequiredService<AbpMassTransitModule>();
            Assert.NotNull(module);
        }

        /// <summary>
        /// Tests that AbpMassTransitOptions is configured correctly
        /// </summary>
        [Fact]
        public void AbpMassTransitOptions_Should_Be_Configured_Correctly()
        {
            // Act
            var options = GetRequiredService<IOptions<AbpMassTransitOptions>>().Value;

            // Assert
            Assert.NotNull(options);
            Assert.Equal("SharpAbp", options.Prefix);
            Assert.NotNull(options.PreConfigures);
            Assert.NotNull(options.PostConfigures);
        }

        /// <summary>
        /// Tests that IMassTransitPublisher is registered correctly
        /// </summary>
        [Fact]
        public void IMassTransitPublisher_Should_Be_Registered_Correctly()
        {
            // Act
            var publisher = GetService<IMassTransitPublisher>();

            // Assert
            Assert.NotNull(publisher);
            Assert.IsType<DefaultMassTransitPublisher>(publisher);
        }

        /// <summary>
        /// Tests that the module configuration is applied correctly through the ABP framework
        /// </summary>
        [Fact]
        public void Module_Should_Apply_Configuration_Through_Framework()
        {
            // Act
            var options = GetRequiredService<IOptions<AbpMassTransitOptions>>().Value;

            // Assert
            Assert.NotNull(options);
            Assert.Equal("SharpAbp", options.Prefix);
            Assert.NotNull(options.PreConfigures);
            Assert.NotNull(options.PostConfigures);
            
            // Verify default timeout values are set
            Assert.True(options.StartTimeoutMilliSeconds > 0);
            Assert.True(options.StopTimeoutMilliSeconds > 0);
        }

        /// <summary>
        /// Tests that the module can be instantiated multiple times
        /// </summary>
        [Fact]
        public void Module_Should_Support_Multiple_Instantiation()
        {
            // Act
            var module1 = new AbpMassTransitModule();
            var module2 = new AbpMassTransitModule();

            // Assert
            Assert.NotNull(module1);
            Assert.NotNull(module2);
            Assert.NotSame(module1, module2);
        }

        /// <summary>
        /// Tests that the module has no explicit dependencies
        /// </summary>
        [Fact]
        public void Module_Should_Have_No_Explicit_Dependencies()
        {
            // Arrange
            var moduleType = typeof(AbpMassTransitModule);

            // Act
            var dependsOnAttribute = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false);

            // Assert
            // The module should have no DependsOn attribute since it has no explicit dependencies
            // AsyncHelper is available from Volo.Abp.Core which is implicitly available through Volo.Abp package
            Assert.Empty(dependsOnAttribute);
        }
    }
}