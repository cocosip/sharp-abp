using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Repository interface for managing RSA credentials with comprehensive query and persistence operations.
    /// Provides specialized methods for RSA key management including identifier-based lookups, random selection,
    /// and advanced filtering capabilities for cryptographic credential storage and retrieval.
    /// </summary>
    public interface IRSACredsRepository : IBasicRepository<RSACreds, Guid>
    {
        /// <summary>
        /// Finds an RSA credential by its unique identifier.
        /// </summary>
        /// <param name="identifier">The unique identifier string to search for. Cannot be null or whitespace.</param>
        /// <param name="includeDetails">Whether to include detailed information in the query result. Default is true.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
        /// <returns>The RSA credential matching the identifier, or null if not found.</returns>
        Task<RSACreds> FindByIdentifierAsync([NotNull] string identifier, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds an RSA credential by identifier while excluding a specific ID from the search results.
        /// This method is useful for validation scenarios where you need to check for duplicate identifiers
        /// while excluding the current entity being updated.
        /// </summary>
        /// <param name="identifier">The unique identifier string to search for.</param>
        /// <param name="expectedId">The ID to exclude from search results. If null, no exclusion is applied.</param>
        /// <param name="includeDetails">Whether to include detailed information in the query result. Default is true.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
        /// <returns>The RSA credential matching the criteria, or null if not found.</returns>
        Task<RSACreds> FindExpectedByIdentifierAsync(string identifier, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a random RSA credential based on optional filtering criteria.
        /// Uses database-level randomization for optimal performance and true randomness.
        /// Useful for load balancing scenarios or when multiple equivalent credentials are available.
        /// </summary>
        /// <param name="sourceType">Optional filter by source type of the credential.</param>
        /// <param name="size">Optional filter by RSA key size in bits (e.g., 1024, 2048, 4096).</param>
        /// <param name="includeDetails">Whether to include detailed information in the query result. Default is true.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
        /// <returns>A randomly selected RSA credential matching the criteria, or null if none found.</returns>
        Task<RSACreds> GetRandomAsync(int? sourceType = null, int? size = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of RSA credentials with optional filtering and sorting.
        /// Supports comprehensive filtering by identifier, source type, and key size.
        /// </summary>
        /// <param name="sorting">Sort expression in string format. If null, defaults to sorting by ID.</param>
        /// <param name="identifier">Optional filter by identifier. Empty string means no filter.</param>
        /// <param name="sourceType">Optional filter by source type of the credential.</param>
        /// <param name="size">Optional filter by RSA key size in bits.</param>
        /// <param name="includeDetails">Whether to include detailed information in the query results. Default is false for performance.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
        /// <returns>A list of RSA credentials matching the specified criteria.</returns>
        Task<List<RSACreds>> GetListAsync(string sorting = null, string identifier = "", int? sourceType = null, int? size = null, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of RSA credentials with optional filtering and sorting.
        /// Implements efficient pagination for large datasets with comprehensive filtering capabilities.
        /// </summary>
        /// <param name="skipCount">Number of records to skip from the beginning.</param>
        /// <param name="maxResultCount">Maximum number of records to return in this page.</param>
        /// <param name="sorting">Sort expression in string format. If null, defaults to sorting by ID.</param>
        /// <param name="identifier">Optional filter by identifier. Empty string means no filter.</param>
        /// <param name="sourceType">Optional filter by source type of the credential.</param>
        /// <param name="size">Optional filter by RSA key size in bits.</param>
        /// <param name="includeDetails">Whether to include detailed information in the query results. Default is false for performance.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
        /// <returns>A paginated list of RSA credentials matching the specified criteria.</returns>
        Task<List<RSACreds>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting = null, string identifier = "", int? sourceType = null, int? size = null, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of RSA credentials matching the specified filter criteria.
        /// Used in conjunction with paged queries to provide total record information for pagination UI.
        /// </summary>
        /// <param name="identifier">Optional filter by identifier. Empty string means no filter.</param>
        /// <param name="sourceType">Optional filter by source type of the credential.</param>
        /// <param name="size">Optional filter by RSA key size in bits.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
        /// <returns>The total number of RSA credentials matching the specified criteria.</returns>
        Task<int> GetCountAsync(string identifier = "", int? sourceType = null, int? size = null, CancellationToken cancellationToken = default);
    }
}
