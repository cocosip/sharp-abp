namespace SharpAbp.Abp.OpenTelemetry
{
    public static class AbpOpenTelemetryOptionsExtensions
    {
        public static AbpOpenTelemetryOptions UseOtlpExporter(
            this AbpOpenTelemetryOptions options,
            OpenTelemetrySignalKinds signalKinds = OpenTelemetrySignalKinds.All)
        {
            return options.UseExporter(OpenTelemetryExporterNames.Otlp, signalKinds);
        }

        public static AbpOpenTelemetryOptions ConfigureLogging(
            this AbpOpenTelemetryOptions options,
            bool includeFormattedMessage = true,
            bool includeScopes = true,
            bool parseStateValues = true,
            bool clearProviders = false)
        {
            options.Logging.IsEnabled = true;
            options.Logging.IncludeFormattedMessage = includeFormattedMessage;
            options.Logging.IncludeScopes = includeScopes;
            options.Logging.ParseStateValues = parseStateValues;
            options.Logging.ClearProviders = clearProviders;
            return options;
        }
    }
}
