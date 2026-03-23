using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.OpenTelemetry
{
    public class AbpOpenTelemetryResourceOptionsTests
    {
        [Fact]
        public void PreConfigure_ShouldBind_ResourceConfiguration()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string?>
            {
                ["OpenTelemetry:Resource:IsEnabled"] = "true",
                ["OpenTelemetry:Resource:ServiceName"] = "new-service",
                ["OpenTelemetry:Resource:ServiceNamespace"] = "new-namespace"
            });

            var options = new AbpOpenTelemetryResourceOptions().PreConfigure(configuration);

            Assert.True(options.IsEnabled);
            Assert.Equal("new-service", options.ServiceName);
            Assert.Equal("new-namespace", options.ServiceNamespace);
        }

        private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();
        }
    }
}
