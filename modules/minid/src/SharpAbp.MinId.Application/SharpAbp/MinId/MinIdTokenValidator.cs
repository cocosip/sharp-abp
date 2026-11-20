using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Implementation of IMinIdTokenValidator that validates MinId authentication tokens.
    /// Checks persisted tokens on every request so revocation cannot be undone by stale cache entries.
    /// </summary>
    public class MinIdTokenValidator : IMinIdTokenValidator, ITransientDependency
    {
        protected IObjectMapper ObjectMapper { get; }
        protected IDistributedCache<MinIdTokenCacheItem> TokenCache { get; }
        protected IMinIdTokenRepository MinIdTokenRepository { get; }

        /// <summary>
        /// Initializes a new instance of the MinIdTokenValidator class.
        /// </summary>
        /// <param name="objectMapper">The object mapper service for mapping between entity and cache item types.</param>
        /// <param name="tokenCache">The distributed cache for storing validated tokens.</param>
        /// <param name="minIdTokenRepository">The repository for accessing MinId token data.</param>
        public MinIdTokenValidator(
            IObjectMapper objectMapper,
            IDistributedCache<MinIdTokenCacheItem> tokenCache,
            IMinIdTokenRepository minIdTokenRepository)
        {
            ObjectMapper = objectMapper;
            TokenCache = tokenCache;
            MinIdTokenRepository = minIdTokenRepository;
        }

        /// <summary>
        /// Asynchronously validates whether a token is valid for a given business type.
        /// Reads persisted token state without trusting positive cache entries.
        /// </summary>
        /// <param name="bizType">The business type identifier associated with the token. Cannot be null, empty, or whitespace.</param>
        /// <param name="token">The authentication token to validate. Cannot be null, empty, or whitespace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>True if the token is valid for the specified business type; otherwise, false.</returns>
        public virtual async Task<bool> ValidateAsync([NotNull] string bizType, [NotNull] string token, CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(bizType, nameof(bizType));
            Check.NotNullOrWhiteSpace(token, nameof(token));

            var minIdToken = await MinIdTokenRepository.FindByTokenAsync(bizType, token, cancellationToken);
            return minIdToken != null && minIdToken.BizType == bizType && minIdToken.Token == token;
        }

        /// <summary>
        /// Asynchronously validates a token for a given business type and throws an exception if invalid.
        /// Reads persisted token state without trusting positive cache entries.
        /// </summary>
        /// <param name="bizType">The business type identifier associated with the token. Cannot be null, empty, or whitespace.</param>
        /// <param name="token">The authentication token to validate. Cannot be null, empty, or whitespace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <exception cref="Volo.Abp.AbpException">Thrown when the token is invalid for the specified business type.
        /// The exception message identifies the business type without exposing the token.</exception>
        public virtual async Task ValidateTokenAsync([NotNull] string bizType, [NotNull] string token, CancellationToken cancellationToken = default)
        {
            if (!await ValidateAsync(bizType, token, cancellationToken))
            {
                throw new AbpAuthorizationException($"Invalid token for business type '{bizType}'.");
            }
        }
    }
}
