using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Faster
{
    /// <summary>
    /// Represents a base interface for FASTER loggers.
    /// </summary>
    public interface IFasterLogger : IDisposable
    {

    }

    /// <summary>
    /// Represents a generic interface for FASTER loggers.
    /// </summary>
    /// <typeparam name="T">The type of objects to be logged.</typeparam>
    public interface IFasterLogger<T> : IFasterLogger where T : class
    {
        /// <summary>
        /// Gets whether the logger has been initialized.
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// Initializes the FASTER logger with the specified configuration.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Writes a single entity to the log.
        /// </summary>
        /// <param name="entity">The entity to write.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The position of the written entry.</returns>
        Task<long> WriteAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Writes a batch of entities to the log.
        /// </summary>
        /// <param name="values">The entities to write.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The positions of the written entries.</returns>
        Task<List<long>> BatchWriteAsync(List<T> values, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads log entries from the pending channel.
        /// </summary>
        /// <param name="count">The number of entries to read.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of log entries.</returns>
        Task<LogEntryList<T>> ReadAsync(int count = 1, CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the specified log entry positions.
        /// </summary>
        /// <param name="entryPosition">The positions to commit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CommitAsync(LogEntryPosition entryPosition, CancellationToken cancellationToken = default);
    }
}