using Microsoft.Extensions.Configuration;

namespace SharpAbp.Abp.OpenTelemetry
{
    public class AbpOpenTelemetryResourceOptions
    {
        public bool IsEnabled { get; set; }

        public string? ServiceName { get; set; }

        public string? ServiceNamespace { get; set; }

        public string? ServiceVersion { get; set; }

        public bool AutoGenerateServiceInstanceId { get; set; }

        public string? ServiceInstanceId { get; set; }

        public AbpOpenTelemetryResourceOptions PreConfigure(IConfiguration configuration)
        {
            var resourceOptions = configuration
                .GetSection("OpenTelemetry:Resource")
                .Get<AbpOpenTelemetryResourceOptions>();

            if (resourceOptions != null)
            {
                IsEnabled = resourceOptions.IsEnabled;
                ServiceName = resourceOptions.ServiceName;
                ServiceNamespace = resourceOptions.ServiceNamespace;
                ServiceVersion = resourceOptions.ServiceVersion;
                AutoGenerateServiceInstanceId = resourceOptions.AutoGenerateServiceInstanceId;
                ServiceInstanceId = resourceOptions.ServiceInstanceId;
            }

            return this;
        }
    }
}
