using JetBrains.Annotations;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace SharpAbp.MinId
{
    public class MinIdTokenValidator : IMinIdTokenValidator, ITransientDependency
    {
        protected IObjectMapper ObjectMapper { get; }
        protected IDistributedCache<MinIdTokenCacheItem> TokenCache { get; }
        protected IMinIdTokenRepository MinIdTokenRepository { get; }
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
        /// Validate token
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<bool> ValidateAsync([NotNull] string bizType, [NotNull] string token)
        {
            Check.NotNullOrWhiteSpace(bizType, nameof(bizType));
            Check.NotNullOrWhiteSpace(token, nameof(token));

            var key = $"{bizType}-{token}";

            var minIdTokenCacheItem = await TokenCache.GetOrAddAsync(key, async () =>
            {
                var minIdToken = await MinIdTokenRepository.FindByTokenAsync(bizType, token);
                return ObjectMapper.Map<MinIdToken, MinIdTokenCacheItem>(minIdToken);
            });

            if (minIdTokenCacheItem != null)
            {
                return minIdTokenCacheItem.Token == token;
            }

            return false;
        }

        /// <summary>
        /// Validate token
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task ValidateTokenAsync([NotNull] string bizType, [NotNull] string token)
        {
            Check.NotNullOrWhiteSpace(bizType, nameof(bizType));
            Check.NotNullOrWhiteSpace(token, nameof(token));

            var key = $"{bizType}-{token}";

            var minIdTokenCacheItem = await TokenCache.GetOrAddAsync(key, async () =>
            {
                var minIdToken = await MinIdTokenRepository.FindByTokenAsync(bizType, token);
                return ObjectMapper.Map<MinIdToken, MinIdTokenCacheItem>(minIdToken);
            });

            if (minIdTokenCacheItem != null)
            {
                return;
            }

            throw new AbpException($"Invalid token");
        }
    }
}
