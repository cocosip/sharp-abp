﻿﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.CryptoVault.EntityFrameworkCore
{
    /// <summary>
    /// Entity Framework Core implementation of <see cref="ISM2CredsRepository"/> for SM2 cryptographic credentials management.
    /// Provides optimized database operations for SM2 elliptic curve credentials including advanced querying, filtering,
    /// pagination, and random selection capabilities. Implements comprehensive data access patterns for secure
    /// SM2 credential storage and retrieval with proper transaction handling, performance optimization, and
    /// Chinese national cryptographic standard compliance. Supports curve-specific operations and efficient
    /// database-level randomization for load balancing scenarios.
    /// </summary>
    public class EfCoreSM2CredsRepository : EfCoreRepository<IAbpCryptoVaultDbContext, SM2Creds, Guid>, ISM2CredsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreSM2CredsRepository"/> class.
        /// </summary>
        /// <param name="dbContextProvider">The database context provider for accessing the crypto vault database context.</param>
        public EfCoreSM2CredsRepository(IDbContextProvider<IAbpCryptoVaultDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        /// <summary>
        /// Finds an SM2 credential by its unique identifier with optimized database query and parameter validation.
        /// Uses indexed lookup for optimal performance and includes comprehensive validation for required parameters.
        /// Essential for credential lookup operations and authentication scenarios requiring specific SM2 key pairs.
        /// </summary>
        /// <param name="identifier">The unique identifier string to search for. Cannot be null or whitespace.</param>
        /// <param name="includeDetails">Whether to include detailed information in the query result for related data loading.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous database operation.</param>
        /// <returns>The SM2 credential matching the identifier, or null if not found.</returns>
        /// <exception cref="ArgumentException">Thrown when identifier is null or whitespace.</exception>
        public virtual async Task<SM2Creds> FindByIdentifierAsync(
            [NotNull] string identifier,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .FirstOrDefaultAsync(x => x.Identifier == identifier, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Finds an SM2 credential by identifier while excluding a specific ID from the search results.
        /// This method is essential for validation scenarios such as checking for duplicate identifiers
        /// during update operations while excluding the current entity being modified. Prevents identifier conflicts
        /// and ensures data integrity in SM2 credential management systems with comprehensive filtering logic.
        /// </summary>
        /// <param name="identifier">The unique identifier string to search for.</param>
        /// <param name="expectedId">The ID to exclude from search results. If null, no exclusion is applied.</param>
        /// <param name="includeDetails">Whether to include detailed information in the query result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous database operation.</param>
        /// <returns>The SM2 credential matching the criteria, or null if not found.</returns>
        public virtual async Task<SM2Creds> FindExpectedByIdentifierAsync(
            string identifier,
            Guid? expectedId = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!identifier.IsNullOrWhiteSpace(), x => x.Identifier == identifier)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a random SM2 credential using database-level randomization for optimal performance and fair distribution.
        /// Implements true randomness through SQL NEWID() function for equal probability selection across all matching credentials.
        /// Supports filtering by source type for domain-specific randomization and curve type for cryptographic compatibility.
        /// Ideal for load balancing scenarios, automatic key rotation, and fair distribution across available SM2 credentials
        /// with specific cryptographic parameters and Chinese national standard compliance.
        /// </summary>
        /// <param name="sourceType">Optional filter by source type of the credential for domain-specific randomization.</param>
        /// <param name="curve">Optional filter by SM2 elliptic curve name (e.g., sm2p256v1, wapip192v1) for cryptographic compatibility.</param>
        /// <param name="includeDetails">Whether to include detailed information in the query result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous database operation.</param>
        /// <returns>A randomly selected SM2 credential matching the criteria, or null if none found.</returns>
        public virtual async Task<SM2Creds> GetRandomAsync(
            int? sourceType = null,
            string curve = "",
            bool includeDetails = true, 
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(!curve.IsNullOrWhiteSpace(), item => item.Curve == curve)
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a comprehensive list of SM2 credentials with flexible filtering and sorting capabilities.
        /// Supports dynamic sorting expressions, multiple filter criteria, and curve-specific filtering for advanced query scenarios.
        /// Optimized for scenarios where full result sets are needed without pagination constraints.
        /// Enables efficient filtering by identifier, source type, and SM2 curve type for precise credential selection
        /// and cryptographic compatibility requirements in Chinese national standard implementations.
        /// </summary>
        /// <param name="sorting">Dynamic LINQ sort expression. If null, defaults to ID-based sorting for consistent results.</param>
        /// <param name="identifier">Optional filter by identifier. Empty string applies no filter for performance optimization.</param>
        /// <param name="sourceType">Optional filter by source type for domain-specific credential retrieval.</param>
        /// <param name="curve">Optional filter by SM2 elliptic curve name for cryptographic compatibility filtering.</param>
        /// <param name="includeDetails">Whether to include detailed information. Default false for list performance optimization.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous database operation.</param>
        /// <returns>A list of SM2 credentials matching the specified criteria with applied sorting.</returns>
        public virtual async Task<List<SM2Creds>> GetListAsync(
            string sorting = null,
            string identifier = "",
            int? sourceType = null,
            string curve = "",
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(!curve.IsNullOrWhiteSpace(), item => item.Curve == curve)
                .OrderBy(sorting ?? nameof(SM2Creds.Id))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a paginated list of SM2 credentials with comprehensive filtering and sorting capabilities.
        /// Implements efficient database-level pagination using SKIP/TAKE for optimal performance with large datasets.
        /// Essential for user interface pagination and API result limiting in SM2 credential management systems.
        /// Supports all filtering options including curve-specific queries for cryptographic compatibility requirements
        /// and Chinese national cryptographic standard compliance. Optimized for high-performance scenarios with
        /// large credential inventories and complex filtering requirements.
        /// </summary>
        /// <param name="skipCount">Number of records to skip from the beginning for pagination offset.</param>
        /// <param name="maxResultCount">Maximum number of records to return in this page for result limiting.</param>
        /// <param name="sorting">Dynamic LINQ sort expression. If null, defaults to ID-based sorting.</param>
        /// <param name="identifier">Optional filter by identifier for targeted searches.</param>
        /// <param name="sourceType">Optional filter by source type for domain-specific pagination.</param>
        /// <param name="curve">Optional filter by SM2 elliptic curve name for cryptographic compatibility filtering.</param>
        /// <param name="includeDetails">Whether to include detailed information. Default false for pagination performance.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous database operation.</param>
        /// <returns>A paginated list of SM2 credentials matching the specified criteria.</returns>
        public virtual async Task<List<SM2Creds>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string identifier = "",
            int? sourceType = null,
            string curve = "",
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(!curve.IsNullOrWhiteSpace(), item => item.Curve == curve)
                .OrderBy(sorting ?? nameof(SM2Creds.Id))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Gets the total count of SM2 credentials matching the specified filter criteria with optimized performance.
        /// Optimized count query without data loading for efficient pagination support and result set analysis.
        /// Essential for calculating total pages and providing accurate result set information to clients.
        /// Supports all filtering criteria including curve-specific counting for cryptographic inventory management
        /// and Chinese national standard compliance reporting. Provides foundation for pagination UI components
        /// and statistical analysis of SM2 credential distribution.
        /// </summary>
        /// <param name="identifier">Optional filter by identifier for targeted counting.</param>
        /// <param name="sourceType">Optional filter by source type for domain-specific counting.</param>
        /// <param name="curve">Optional filter by SM2 elliptic curve name for curve-specific inventory counting.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous database operation.</param>
        /// <returns>The total number of SM2 credentials matching the specified criteria.</returns>
        public virtual async Task<int> GetCountAsync(
            string identifier = "",
            int? sourceType = null,
            string curve = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(!curve.IsNullOrWhiteSpace(), item => item.Curve == curve)
                .CountAsync(GetCancellationToken(cancellationToken));
        }
    }
}
