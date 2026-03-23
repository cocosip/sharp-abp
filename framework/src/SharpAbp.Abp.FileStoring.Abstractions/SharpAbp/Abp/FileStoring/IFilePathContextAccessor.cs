using System;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Provides ambient access to the current <see cref="FilePathContext"/>.
    /// <para>
    /// Inject this interface to set per-operation path parameters before calling file operations:
    /// <code>
    /// using (_filePathContextAccessor.Change(new FilePathContext { TenantCode = "my-org" }))
    /// {
    ///     await fileContainer.SaveAsync(fileId, stream, ext);
    /// }
    /// </code>
    /// </para>
    /// </summary>
    public interface IFilePathContextAccessor
    {
        /// <summary>Gets or sets the current path context.</summary>
        FilePathContext? Current { get; set; }

        /// <summary>
        /// Sets <paramref name="context"/> as the current context and returns a disposable
        /// that restores the previous context when disposed.
        /// </summary>
        IDisposable Change(FilePathContext? context);
    }
}
