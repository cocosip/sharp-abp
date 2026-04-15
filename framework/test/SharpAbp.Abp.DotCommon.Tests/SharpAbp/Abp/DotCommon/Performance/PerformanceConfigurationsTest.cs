using SharpAbp.Abp.DotCommon.Performance;
using Xunit;

namespace SharpAbp.Abp.DotCommon
{
    public class PerformanceConfigurationsTest
    {
        [Fact]
        public void GetConfiguration_Should_Return_Default_When_Name_Does_Not_Exist()
        {
            var configurations = new PerformanceConfigurations();

            var configuration = configurations.GetConfiguration("missing");

            Assert.NotNull(configuration);
            Assert.True(configuration.AutoLogging);
            Assert.Equal(1, configuration.StatIntervalSeconds);
        }

        [Fact]
        public void Configure_Should_Create_And_Update_Named_Configuration()
        {
            var configurations = new PerformanceConfigurations();

            configurations.Configure("orders", configuration =>
            {
                configuration.AutoLogging = false;
                configuration.StatIntervalSeconds = 5;
            });

            var configuration = configurations.GetConfiguration("orders");

            Assert.False(configuration.AutoLogging);
            Assert.Equal(5, configuration.StatIntervalSeconds);
        }

        [Fact]
        public void ConfigureAll_Should_Visit_Default_And_Custom_Configurations()
        {
            var configurations = new PerformanceConfigurations();
            var count = 0;

            configurations.Configure("orders", _ => { });
            configurations.Configure("payments", _ => { });

            configurations.ConfigureAll((_, _) => count++);

            Assert.Equal(3, count);
        }
    }
}
