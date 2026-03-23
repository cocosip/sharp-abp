namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Builds the storage path (object key) for a file operation.
    /// <para>
    /// All default <c>FileNameCalculator</c> implementations delegate to this service,
    /// so replacing or configuring it affects all storage providers at once.
    /// </para>
    /// <para>
    /// Use <see cref="AbpFileStoringAbstractionsOptions.FilePathBuilder"/> for global rules,
    /// and <see cref="IFilePathContextAccessor"/> to supply per-operation parameters.
    /// </para>
    /// </summary>
    public interface IFilePathBuilder
    {
        /// <summary>Computes and returns the storage path for the given file operation arguments.</summary>
        string Build(FileProviderArgs args);
    }
}
