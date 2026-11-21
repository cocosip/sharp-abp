﻿using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Base class for file provider implementations.
    /// Provides common functionality and defines the contract for file operations.
    /// </summary>
    public abstract class FileProviderBase : IFileProvider
    {
        /// <summary>
        /// Gets the unique identifier for this file provider.
        /// </summary>
        public abstract string Provider { get; }

        /// <summary>
        /// Saves a file asynchronously.
        /// </summary>
        /// <param name="args">The arguments containing file data and metadata.</param>
        /// <returns>The unique identifier of the saved file.</returns>
        public abstract Task<string> SaveAsync(FileProviderSaveArgs args);

        /// <summary>
        /// Deletes a file asynchronously.
        /// </summary>
        /// <param name="args">The arguments identifying the file to delete.</param>
        /// <returns>True if the file was successfully deleted, false otherwise.</returns>
        public abstract Task<bool> DeleteAsync(FileProviderDeleteArgs args);

        /// <summary>
        /// Checks if a file exists asynchronously.
        /// </summary>
        /// <param name="args">The arguments identifying the file to check.</param>
        /// <returns>True if the file exists, false otherwise.</returns>
        public abstract Task<bool> ExistsAsync(FileProviderExistsArgs args);

        /// <summary>
        /// Downloads a file asynchronously.
        /// </summary>
        /// <param name="args">The arguments identifying the file to download and the destination path.</param>
        /// <returns>True if the file was successfully downloaded, false otherwise.</returns>
        public abstract Task<bool> DownloadAsync(FileProviderDownloadArgs args);

        /// <summary>
        /// Gets a file stream asynchronously.
        /// </summary>
        /// <param name="args">The arguments identifying the file to retrieve.</param>
        /// <returns>A stream containing the file data, or null if the file doesn't exist.</returns>
        public abstract Task<Stream?> GetOrNullAsync(FileProviderGetArgs args);

        /// <summary>
        /// Gets an access URL for a file asynchronously.
        /// </summary>
        /// <param name="args">The arguments identifying the file to access.</param>
        /// <returns>A URL that can be used to access the file.</returns>
        public abstract Task<string> GetAccessUrlAsync(FileProviderAccessArgs args);

        /// <summary>
        /// Attempts to copy a stream to a memory stream asynchronously.
        /// </summary>
        /// <param name="stream">The source stream to copy from.</param>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
        /// <returns>A memory stream containing the copied data, or null if the source stream is null.</returns>
        protected virtual async Task<Stream?> TryCopyToMemoryStreamAsync(
            Stream stream, 
            CancellationToken cancellationToken = default)
        {
            if (stream == null)
            {
                return null;
            }

            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        protected virtual async Task<int> ReadToBufferAsync(
            Stream stream,
            byte[] buffer,
            int count,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(stream, nameof(stream));
            Check.NotNull(buffer, nameof(buffer));

            var totalBytesRead = 0;
            while (totalBytesRead < count)
            {
                var bytesRead = await stream.ReadAsync(
                    buffer,
                    totalBytesRead,
                    count - totalBytesRead,
                    cancellationToken);

                if (bytesRead == 0)
                {
                    break;
                }

                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }

        /// <summary>
        /// Attempts to write a stream to a file asynchronously.
        /// </summary>
        /// <param name="stream">The source stream to write from.</param>
        /// <param name="path">The file path to write to.</param>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual async Task TryWriteToFileAsync(
            Stream stream,
            [NotNull] string path,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(stream, nameof(stream));
            await TryWriteToFileAsync(path, async (output, token) =>
            {
                await stream.CopyToAsync(output, token);
                return true;
            }, cancellationToken);
        }

        protected async Task<bool> TryWriteToFileAsync(
            string path,
            Func<Stream, CancellationToken, Task<bool>> writeAsync,
            CancellationToken cancellationToken)
        {
            Check.NotNullOrWhiteSpace(path, nameof(path));

            cancellationToken.ThrowIfCancellationRequested();
            var destination = Path.GetFullPath(path);
            var directory = Path.GetDirectoryName(destination) ?? throw new IOException("The download destination must be a file path.");
            var canReplace = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (!canReplace && File.Exists(destination))
            {
                throw UnixReplacementNotSupported();
            }

            Directory.CreateDirectory(directory);
            var stagingPath = Path.Combine(directory, $".sharpabp-{Guid.NewGuid():N}.tmp");
            var stagingCreated = false;

            try
            {
                using (var output = new FileStream(stagingPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
                {
                    stagingCreated = true;
                    if (!await writeAsync(output, cancellationToken))
                    {
                        return false;
                    }

                    await output.FlushAsync(cancellationToken);
                }

                cancellationToken.ThrowIfCancellationRequested();
                if (!canReplace)
                {
                    // POSIX link publishes the staged file atomically and fails if the destination exists.
                    if (Link(stagingPath, destination) != 0)
                    {
                        var error = Marshal.GetLastWin32Error();
                        if (File.Exists(destination))
                        {
                            throw UnixReplacementNotSupported();
                        }
                        throw new IOException("The download could not be published as a new file.", new Win32Exception(error));
                    }
                    return true;
                }

                try
                {
                    File.Move(stagingPath, destination);
                }
                catch (IOException) when (File.Exists(destination))
                {
                    File.Replace(stagingPath, destination, null);
                }

                stagingCreated = false;
                return true;
            }
            finally
            {
                if (stagingCreated)
                {
                    try
                    {
                        File.Delete(stagingPath);
                    }
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        Trace.TraceWarning("Failed to remove temporary download file '{0}': {1}", stagingPath, ex.Message);
                    }
                }
            }
        }

        private static NotSupportedException UnixReplacementNotSupported() =>
            new NotSupportedException("Replacing an existing download on this platform cannot preserve its ownership and access controls.");

        // CharSet.Ansi marshals strings as UTF-8 on Unix and is available on .NET Standard 2.0.
        [DllImport("libc", EntryPoint = "link", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern int Link(string existingPath, string newPath);
    }
}
