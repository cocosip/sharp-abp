using System;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Configures how <see cref="IFilePathBuilder"/> constructs storage paths.
    /// Add to <see cref="AbpFileStoringAbstractionsOptions.FilePathBuilder"/>.
    /// </summary>
    public class FilePathBuilderOptions
    {
        /// <summary>
        /// Static global path prefix, e.g. "uploads" or "prod".
        /// Ignored when <see cref="PrefixFactory"/> is set.
        /// Can be overridden per-operation via <see cref="FilePathContext.Prefix"/>.
        /// </summary>
        public string? Prefix { get; set; }

        /// <summary>
        /// Segment name used for host (non-tenant) paths. Default: "host".
        /// </summary>
        public string HostSegment { get; set; } = "host";

        /// <summary>
        /// Segment name used as the tenants directory. Default: "tenants".
        /// </summary>
        public string TenantsSegment { get; set; } = "tenants";

        /// <summary>
        /// Custom factory to determine the tenant identifier string used in the path.
        /// <para>Parameters: (tenantId, tenantName, currentContext)</para>
        /// <para>Returns: the string placed after <see cref="TenantsSegment"/>, e.g. tenant code or alias.</para>
        /// <para>Default (when null): <c>tenantId.ToString("D")</c></para>
        /// </summary>
        /// <example>
        /// // Use tenant Name as path segment
        /// options.FilePathBuilder.TenantIdentifierFactory = (id, name, ctx) =>
        ///     ctx?.TenantCode ?? name ?? id.ToString("D");
        /// </example>
        public Func<Guid, string?, FilePathContext?, string>? TenantIdentifierFactory { get; set; }

        /// <summary>
        /// Dynamic prefix factory. When set, takes precedence over <see cref="Prefix"/> and
        /// <see cref="FilePathContext.Prefix"/>.
        /// <para>Parameter: currentContext (may be null when no context is active)</para>
        /// <para>Returns: prefix string, or null/empty for no prefix.</para>
        /// </summary>
        /// <example>
        /// options.FilePathBuilder.PrefixFactory = ctx =>
        ///     ctx?.Extra.GetValueOrDefault("region") as string ?? "default";
        /// </example>
        public Func<FilePathContext?, string?>? PrefixFactory { get; set; }
    }
}
