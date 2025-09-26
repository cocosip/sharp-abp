using JetBrains.Annotations;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Provides an interface for selecting and retrieving file providers based on container names.
    /// </summary>
    public interface IFileProviderSelector
    {
        /// <summary>
        /// Gets the file provider for the specified container name.
        /// </summary>
        /// <param name="containerName">The name of the container for which to retrieve the file provider.</param>
        /// <returns>The file provider associated with the specified container name.</returns>
        /// <exception cref="Volo.Abp.AbpException">
        /// Thrown when no container configuration is found for the specified container name.
        /// </exception>
        [NotNull]
        IFileProvider Get([NotNull] string containerName);
    }
}