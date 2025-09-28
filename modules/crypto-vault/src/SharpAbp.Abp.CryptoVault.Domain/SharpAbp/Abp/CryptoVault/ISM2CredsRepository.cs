using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Repository interface for managing SM2 cryptographic credentials with comprehensive query and persistence operations.
    /// Provides specialized methods for SM2 elliptic curve key management including identifier-based lookups, curve-specific filtering,
    /// random selection capabilities, and advanced querying for Chinese national cryptographic standard compliance.
    /// Supports efficient data access patterns for SM2 credential storage, retrieval, and validation operations.
    /// </summary>
    public interface ISM2CredsRepository : IBasicRepository<SM2Creds, Guid>
    {
        /// <summary>
        /// Finds an SM2 credential by its unique identifier with optimized query performance.
        /// Essential for credential lookup operations and authentication scenarios requiring specific SM2 key pairs.
        /// </summary>
        /// <param name="identifier">The unique identifier string to search for. Cannot be null or whitespace.</param>
        /// <param name="includeDetails">Whether to include detailed information in the query result for related data loading. Default is true.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
        /// <returns>The SM2 credential matching the identifier, or null if not found.</returns>
        Task<SM2Creds> FindByIdentifierAsync([NotNull] string identifier, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds an SM2 credential by identifier while excluding a specific ID from the search results.
        /// This method is crucial for validation scenarios such as checking for duplicate identifiers
        /// during update operations while excluding the current entity being modified. Prevents identifier conflicts
        /// and ensures data integrity in SM2 credential management systems.
        /// </summary>
        /// <param name="identifier">The unique identifier string to search for.</param>
        /// <param name="expectedId">The ID to exclude from search results. If null, no exclusion is applied.</param>
        /// <param name="includeDetails">Whether to include detailed information in the query result. Default is true.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
        /// <returns>The SM2 credential matching the criteria, or null if not found.</returns>
        Task<SM2Creds> FindExpectedByIdentifierAsync(string identifier, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a random SM2 credential based on optional filtering criteria using database-level randomization.
        /// Implements true randomness for load balancing scenarios and fair distribution across available SM2 credentials.
        /// Supports filtering by source type for domain-specific selection and curve type for cryptographic compatibility.
        /// Ideal for scenarios requiring automatic SM2 key selection with specific cryptographic parameters.
        /// </summary>
        /// <param name="sourceType">Optional filter by source type of the credential for domain-specific randomization.</param>
        /// <param name="curve">Optional filter by SM2 elliptic curve name (e.g., sm2p256v1, wapip192v1). Empty string means no filter.</param>
        /// <param name="includeDetails">Whether to include detailed information in the query result. Default is true.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
        /// <returns>A randomly selected SM2 credential matching the criteria, or null if none found.</returns>
        Task<SM2Creds> GetRandomAsync(int? sourceType = null, string curve = "", bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a comprehensive list of SM2 credentials with flexible filtering and sorting capabilities.
        /// Supports dynamic sorting expressions and multiple filter criteria for advanced query scenarios.
        /// Enables filtering by identifier, source type, and SM2 curve type for precise credential selection.
        /// Optimized for scenarios where full result sets are needed without pagination constraints.
        /// </summary>
        /// <param name="sorting">Dynamic LINQ sort expression. If null, defaults to ID-based sorting for consistent results.</param>
        /// <param name="identifier">Optional filter by identifier. Empty string applies no filter for performance optimization.</param>
        /// <param name="sourceType">Optional filter by source type for domain-specific credential retrieval.</param>
        /// <param name="curve">Optional filter by SM2 elliptic curve name. Empty string means no curve-specific filtering.</param>
        /// <param name="includeDetails">Whether to include detailed information. Default false for list performance optimization.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
        /// <returns>A list of SM2 credentials matching the specified criteria with applied sorting.</returns>
        Task<List<SM2Creds>> GetListAsync(string sorting = null, string identifier = "", int? sourceType = null, string curve = "", bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of SM2 credentials with comprehensive filtering and sorting capabilities.
        /// Implements efficient database-level pagination using SKIP/TAKE for optimal performance with large datasets.
        /// Essential for user interface pagination and API result limiting in SM2 credential management systems.
        /// Supports all filtering options including curve-specific queries for cryptographic compatibility requirements.
        /// </summary>
        /// <param name="skipCount">Number of records to skip from the beginning for pagination offset.</param>
        /// <param name="maxResultCount">Maximum number of records to return in this page for result limiting.</param>
        /// <param name="sorting">Dynamic LINQ sort expression. If null, defaults to ID-based sorting.</param>
        /// <param name="identifier">Optional filter by identifier for targeted searches.</param>
        /// <param name="sourceType">Optional filter by source type for domain-specific pagination.</param>
        /// <param name="curve">Optional filter by SM2 elliptic curve name for cryptographic compatibility filtering.</param>
        /// <param name="includeDetails">Whether to include detailed information. Default false for pagination performance.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
        /// <returns>A paginated list of SM2 credentials matching the specified criteria.</returns>
        Task<List<SM2Creds>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting = null, string identifier = "", int? sourceType = null, string curve = "", bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of SM2 credentials matching the specified filter criteria.
        /// Optimized count query without data loading for efficient pagination support and result set analysis.
        /// Essential for calculating total pages and providing accurate result set information to clients.
        /// Supports all filtering criteria including curve-specific counting for cryptographic inventory management.
        /// </summary>
        /// <param name="identifier">Optional filter by identifier for targeted counting.</param>
        /// <param name="sourceType">Optional filter by source type for domain-specific counting.</param>
        /// <param name="curve">Optional filter by SM2 elliptic curve name for curve-specific inventory counting.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
        /// <returns>The total number of SM2 credentials matching the specified criteria.</returns>
        Task<int> GetCountAsync(string identifier = "", int? sourceType = null, string curve = "", CancellationToken cancellationToken = default);
    }
}
