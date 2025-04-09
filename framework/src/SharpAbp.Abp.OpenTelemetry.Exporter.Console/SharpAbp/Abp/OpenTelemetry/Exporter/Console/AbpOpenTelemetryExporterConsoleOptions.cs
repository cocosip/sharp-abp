using Microsoft.Extensions.Configuration;
using OpenTelemetry.Exporter;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Console
{
    public class AbpOpenTelemetryExporterConsoleOptions
    {
        public string? Name { get; set; }
        public ConsoleExporterOutputTargets Targets { get; set; }

        public AbpOpenTelemetryExporterConsoleOptions PreConfigure(IConfiguration configuration)
        {
            var openTelemetryExporterConsoleOptions = configuration
                .GetSection("OpenTelemetryOptions:Exporters:Console")
                .Get<AbpOpenTelemetryExporterConsoleOptions>();

            if (openTelemetryExporterConsoleOptions != null)
            {
                Name = openTelemetryExporterConsoleOptions.Name;
                Targets = openTelemetryExporterConsoleOptions.Targets;
            }
            return this;
        }
    }
}
