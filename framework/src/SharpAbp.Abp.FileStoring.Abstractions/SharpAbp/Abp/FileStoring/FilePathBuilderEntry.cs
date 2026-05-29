namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Represents the JSON-deserializable configuration for path building.
    /// Maps to the <c>FileStoringOptions:FilePathBuilder</c> section in appsettings.json.
    /// All properties are nullable so that only explicitly-set values override defaults.
    /// </summary>
    public class FilePathBuilderEntry
    {
        /// <summary>
        /// Overall path building strategy.
        /// Maps to <see cref="AbpFileStoringAbstractionsOptions.FilePathStrategy"/>.
        /// </summary>
        public FilePathGenerationStrategy? FilePathStrategy { get; set; }

        /// <summary>
        /// Static path prefix applied to all generated paths.
        /// Maps to <see cref="FilePathBuilderOptions.Prefix"/>.
        /// </summary>
        public string? Prefix { get; set; }

        /// <summary>
        /// Override the host segment name (default: "host").
        /// Maps to <see cref="FilePathBuilderOptions.HostSegment"/>.
        /// </summary>
        public string? HostSegment { get; set; }

        /// <summary>
        /// Override the tenants directory segment name (default: "tenants").
        /// Maps to <see cref="FilePathBuilderOptions.TenantsSegment"/>.
        /// </summary>
        public string? TenantsSegment { get; set; }

        /// <summary>
        /// Selects the built-in tenant identifier strategy.
        /// Maps to <see cref="FilePathBuilderOptions.TenantIdentifierFactory"/> via a built-in factory.
        /// Use <see cref="TenantIdentifierMode.TenantId"/> (default) for GUID-based paths,
        /// <see cref="TenantIdentifierMode.TenantName"/> to use the tenant Name,
        /// or <see cref="TenantIdentifierMode.TenantCode"/> to use <see cref="FilePathContext.TenantCode"/>.
        /// </summary>
        public TenantIdentifierMode? TenantIdentifierMode { get; set; }

        public virtual void ApplyTo(AbpFileStoringAbstractionsOptions options)
        {
            if (FilePathStrategy.HasValue)
            {
                options.FilePathStrategy = FilePathStrategy.Value;
            }

            if (!string.IsNullOrEmpty(Prefix))
            {
                options.FilePathBuilder.Prefix = Prefix;
            }

            if (HostSegment != null)
            {
                options.FilePathBuilder.HostSegment = HostSegment;
            }

            if (TenantsSegment != null)
            {
                options.FilePathBuilder.TenantsSegment = TenantsSegment;
            }

            if (!TenantIdentifierMode.HasValue)
            {
                return;
            }

            if (TenantIdentifierMode.Value == FileStoring.TenantIdentifierMode.TenantCode)
            {
                options.FilePathBuilder.TenantIdentifierFactory = (id, name, ctx) =>
                    ctx?.TenantCode ?? id.ToString("D");
            }
            else if (TenantIdentifierMode.Value == FileStoring.TenantIdentifierMode.TenantName)
            {
                options.FilePathBuilder.TenantIdentifierFactory = (id, name, ctx) =>
                    ctx?.TenantCode
                    ?? (!string.IsNullOrEmpty(name) ? name! : id.ToString("D"));
            }
            else
            {
                options.FilePathBuilder.TenantIdentifierFactory = null;
            }
        }
    }
}
