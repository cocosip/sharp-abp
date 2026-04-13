#nullable enable

using Xunit;

namespace SharpAbp.Abp.Faster
{
    public class AbpFasterConfigurationsTests
    {
        [Fact]
        public void ConfigureDefault_ShouldCreateDefaultConfigurationWithoutRecursion()
        {
            var configurations = new AbpFasterConfigurations();

            configurations.ConfigureDefault(c => c.FileName = "default.log");

            var configuration = configurations.GetConfiguration("missing");

            Assert.Equal("default.log", configuration.FileName);
            Assert.Same(configuration, configurations.GetConfiguration(DefaultFasterLog.Name));
        }
    }
}
