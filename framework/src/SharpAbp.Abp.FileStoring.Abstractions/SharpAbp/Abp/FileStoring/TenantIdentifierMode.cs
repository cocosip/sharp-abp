namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Determines which tenant property is used as the path segment
    /// when <see cref="FilePathGenerationStrategy.TenantBased"/> is active
    /// and no custom <see cref="FilePathBuilderOptions.TenantIdentifierFactory"/> is set.
    /// </summary>
    public enum TenantIdentifierMode
    {
        /// <summary>
        /// Use the tenant GUID formatted as "D" (e.g. <c>3fa85f64-5717-4562-b3fc-2c963f66afa6</c>).
        /// This is the default and maintains backward compatibility.
        /// </summary>
        TenantId = 0,

        /// <summary>
        /// Use the tenant <c>Name</c> as the path segment (e.g. <c>my-org</c>).
        /// Falls back to <see cref="TenantId"/> format when Name is null or empty.
        /// </summary>
        TenantName = 1
    }
}
