using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoring
{
    public interface IFileContainer<TContainer> : IFileContainer
        where TContainer: class
    {
        
    }

    public interface IFileContainer
    {
        /// <summary>
        /// Saves a file <see cref="Stream"/> to the container.
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="stream">A stream for the file</param>
        /// <param name="overrideExisting">
        /// Set <code>true</code> to override if there is already a file in the container with the given name.
        /// If set to <code>false</code> (default), throws exception if there is already a file in the container with the given name.
        /// </param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task SaveAsync(
            string name,
            Stream stream,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default
        );
        
        /// <summary>
        /// Deletes a file from the container.
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Returns true if actually deleted the file.
        /// Returns false if the file with the given <paramref name="name"/> was not exists.  
        /// </returns>
        Task<bool> DeleteAsync(
            string name,
            CancellationToken cancellationToken = default
        );
        
        /// <summary>
        /// Checks if a file does exists in the container.
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task<bool> ExistsAsync(
            string name,
            CancellationToken cancellationToken = default
        );
        
        /// <summary>
        /// Gets a file from the container.
        /// It actually gets a <see cref="Stream"/> to read the file data.
        /// It throws exception if there is no file with the given <paramref name="name"/>.
        /// Use <see cref="GetOrNullAsync"/> if you want to get <code>null</code> if there is no file with the given <paramref name="name"/>. 
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// A <see cref="Stream"/> to read the file data.
        /// </returns>
        Task<Stream> GetAsync(
            string name,
            CancellationToken cancellationToken = default
        );
        
        /// <summary>
        /// Gets a file from the container, or returns null if there is no file with the given <paramref name="name"/>.
        /// It actually gets a <see cref="Stream"/> to read the file data.
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// A <see cref="Stream"/> to read the file data.
        /// </returns>
        Task<Stream> GetOrNullAsync(
            string name,
            CancellationToken cancellationToken = default
        );
        
        //TODO: Create shortcut extension methods: GetAsArraryAsync, GetAsStringAsync(encoding) (and null versions)
    }
}