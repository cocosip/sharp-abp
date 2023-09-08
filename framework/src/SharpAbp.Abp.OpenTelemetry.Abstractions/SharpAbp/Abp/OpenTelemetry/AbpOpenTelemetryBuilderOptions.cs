using Microsoft.Extensions.Configuration;

namespace SharpAbp.Abp.OpenTelemetry
{
    public class AbpOpenTelemetryBuilderOptions
    {
        public bool IsEnabled { get; set; }
        public string ServiceName { get; set; }
        public string ServiceVersion { get; set; }
        public string ServiceInstanceId { get; set; }

        public AbpOpenTelemetryBuilderOptions PreConfigure(IConfiguration configuration)
        {
            var openTelemetryBuilderOptions = configuration
                .GetSection("OpenTelemetryBuilderOptions")
                .Get<AbpOpenTelemetryBuilderOptions>();

            if (openTelemetryBuilderOptions != null)
            {
                IsEnabled = openTelemetryBuilderOptions.IsEnabled;
                ServiceName = openTelemetryBuilderOptions.ServiceName;
                ServiceVersion = openTelemetryBuilderOptions.ServiceVersion;
                ServiceInstanceId = openTelemetryBuilderOptions.ServiceInstanceId;
            }

            return this;
        }
    }
}
