using Microsoft.Extensions.Options;
using SharpAbp.Abp.DotCommon.Performance;
using Xunit;

namespace SharpAbp.Abp.DotCommon
{
    public class DefaultPerformanceConfigurationProviderTest
    {
        [Fact]
        public void Get_Should_Return_Configured_Performance_Configuration()
        {
            var options = new AbpPerformanceOptions();
            options.Configurations.Configure("orders", configuration =>
            {
                configuration.StatIntervalSeconds = 3;
                configuration.AutoLogging = false;
            });

            var provider = new DefaultPerformanceConfigurationProvider(Options.Create(options));

            var configuration = provider.Get("orders");

            Assert.Equal(3, configuration.StatIntervalSeconds);
            Assert.False(configuration.AutoLogging);
        }

        [Fact]
        public void Get_Should_Fallback_To_Default_Configuration()
        {
            var provider = new DefaultPerformanceConfigurationProvider(Options.Create(new AbpPerformanceOptions()));

            var configuration = provider.Get("missing");

            Assert.True(configuration.AutoLogging);
            Assert.Equal(1, configuration.StatIntervalSeconds);
        }
    }
}
