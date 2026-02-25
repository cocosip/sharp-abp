using System.Collections.Generic;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Holds per-operation path building parameters that can be passed in externally.
    /// Inject <see cref="IFilePathContextAccessor"/> and call Change() to set this before file operations.
    /// </summary>
    public class FilePathContext
    {
        /// <summary>
        /// Custom tenant identifier used in the path (e.g. tenant code or alias).
        /// Overrides the default tenant ID when <see cref="FilePathBuilderOptions.TenantIdentifierFactory"/> uses it.
        /// </summary>
        public string? TenantCode { get; set; }

        /// <summary>
        /// Path prefix prepended before the tenant/host segment.
        /// Overrides <see cref="FilePathBuilderOptions.Prefix"/> when set.
        /// </summary>
        public string? Prefix { get; set; }

        /// <summary>
        /// Additional arbitrary key-value parameters available to factory delegates.
        /// </summary>
        public Dictionary<string, object?> Extra { get; set; } = [];
    }
}
