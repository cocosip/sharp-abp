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
        /// or <see cref="TenantIdentifierMode.TenantName"/> to use the tenant Name.
        /// </summary>
        public TenantIdentifierMode? TenantIdentifierMode { get; set; }
    }
}
