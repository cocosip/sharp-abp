using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace SharpAbp.Abp.CryptoVault.EntityFrameworkCore
{
    /// <summary>
    /// Entity Framework Core implementation of <see cref="IRSACredsRepository"/> for RSA credentials management.
    /// Provides optimized database operations for RSA cryptographic credentials including advanced querying,
    /// filtering, pagination, and random selection capabilities. Implements comprehensive data access patterns
    /// for secure credential storage and retrieval with proper transaction handling and performance optimization.
    /// </summary>
    public class EfCoreRSACredsRepository : EfCoreRepository<IAbpCryptoVaultDbContext, RSACreds, Guid>, IRSACredsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRSACredsRepository"/> class.
        /// </summary>
        /// <param name="dbContextProvider">The database context provider for accessing the crypto vault database context.</param>
        public EfCoreRSACredsRepository(IDbContextProvider<IAbpCryptoVaultDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        /// <summary>
        /// Finds an RSA credential by its unique identifier with optimized database query.
        /// Uses indexed lookup for optimal performance and includes validation for required parameters.
        /// </summary>
        /// <param name="identifier">The unique identifier string to search for. Cannot be null or whitespace.</param>
        /// <param name="includeDetails">Whether to include detailed information in the query result for related data loading.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous database operation.</param>
        /// <returns>The RSA credential matching the identifier, or null if not found.</returns>
        /// <exception cref="ArgumentException">Thrown when identifier is null or whitespace.</exception>
        public virtual async Task<RSACreds> FindByIdentifierAsync(
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
        /// Finds an RSA credential by identifier while excluding a specific ID from the search results.
        /// This method is essential for validation scenarios such as checking for duplicate identifiers
        /// during update operations while excluding the current entity being modified.
        /// </summary>
        /// <param name="identifier">The unique identifier string to search for.</param>
        /// <param name="expectedId">The ID to exclude from search results. If null, no exclusion is applied.</param>
        /// <param name="includeDetails">Whether to include detailed information in the query result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous database operation.</param>
        /// <returns>The RSA credential matching the criteria, or null if not found.</returns>
        public virtual async Task<RSACreds> FindExpectedByIdentifierAsync(
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
        /// Retrieves a random RSA credential using database-level randomization for optimal performance.
        /// Implements true randomness through SQL NEWID() function for fair distribution across credentials.
        /// Supports filtering by source type and key size for targeted random selection scenarios.
        /// </summary>
        /// <param name="sourceType">Optional filter by source type of the credential for domain-specific randomization.</param>
        /// <param name="size">Optional filter by RSA key size in bits (e.g., 1024, 2048, 4096) for security level matching.</param>
        /// <param name="includeDetails">Whether to include detailed information in the query result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous database operation.</param>
        /// <returns>A randomly selected RSA credential matching the criteria, or null if none found.</returns>
        public virtual async Task<RSACreds> GetRandomAsync(
            int? sourceType = null,
            int? size = null,
            bool includeDetails = true, 
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(size.HasValue, item => item.Size == size.Value)
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a comprehensive list of RSA credentials with flexible filtering and sorting capabilities.
        /// Supports dynamic sorting expressions and multiple filter criteria for advanced query scenarios.
        /// Optimized for scenarios where full result sets are needed without pagination constraints.
        /// </summary>
        /// <param name="sorting">Dynamic LINQ sort expression. If null, defaults to ID-based sorting for consistent results.</param>
        /// <param name="identifier">Optional filter by identifier. Empty string applies no filter for performance.</param>
        /// <param name="sourceType">Optional filter by source type for domain-specific credential retrieval.</param>
        /// <param name="size">Optional filter by RSA key size for security level-specific queries.</param>
        /// <param name="includeDetails">Whether to include detailed information. Default false for list performance optimization.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous database operation.</param>
        /// <returns>A list of RSA credentials matching the specified criteria with applied sorting.</returns>
        public virtual async Task<List<RSACreds>> GetListAsync(
            string sorting = null,
            string identifier = "",
            int? sourceType = null,
            int? size = null,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(size.HasValue, item => item.Size == size.Value)
                .OrderBy(sorting ?? nameof(RSACreds.Id))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a paginated list of RSA credentials with comprehensive filtering and sorting.
        /// Implements efficient database-level pagination using SKIP/TAKE for optimal performance
        /// with large datasets. Essential for user interface pagination and API result limiting.
        /// </summary>
        /// <param name="skipCount">Number of records to skip from the beginning for pagination offset.</param>
        /// <param name="maxResultCount">Maximum number of records to return in this page for result limiting.</param>
        /// <param name="sorting">Dynamic LINQ sort expression. If null, defaults to ID-based sorting.</param>
        /// <param name="identifier">Optional filter by identifier for targeted searches.</param>
        /// <param name="sourceType">Optional filter by source type for domain-specific pagination.</param>
        /// <param name="size">Optional filter by RSA key size for security level-specific pagination.</param>
        /// <param name="includeDetails">Whether to include detailed information. Default false for pagination performance.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous database operation.</param>
        /// <returns>A paginated list of RSA credentials matching the specified criteria.</returns>
        public virtual async Task<List<RSACreds>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string identifier = "",
            int? sourceType = null,
            int? size = null,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(size.HasValue, item => item.Size == size.Value)
                .OrderBy(sorting ?? nameof(RSACreds.Id))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Gets the total count of RSA credentials matching the specified filter criteria.
        /// Optimized count query without data loading for efficient pagination support.
        /// Essential for calculating total pages and providing accurate result set information to clients.
        /// </summary>
        /// <param name="identifier">Optional filter by identifier for targeted counting.</param>
        /// <param name="sourceType">Optional filter by source type for domain-specific counting.</param>
        /// <param name="size">Optional filter by RSA key size for security level-specific counting.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the asynchronous database operation.</param>
        /// <returns>The total number of RSA credentials matching the specified criteria.</returns>
        public virtual async Task<int> GetCountAsync(
            string identifier = "",
            int? sourceType = null,
            int? size = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(size.HasValue, item => item.Size == size.Value)
                .CountAsync(GetCancellationToken(cancellationToken));
        }
    }
}
