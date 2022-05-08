using JetBrains.Annotations;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace SharpAbp.MinId
{
    public class MinIdInfoQuerier : IMinIdInfoQuerier, ITransientDependency
    {
        protected IDistributedCache<MinIdInfoCacheItem> MinIdInfoCache { get; }
        protected IMinIdInfoRepository MinIdInfoRepository { get; }

        public MinIdInfoQuerier(
            IObjectMapper objectMapper,
            IDistributedCache<MinIdInfoCacheItem> minIdInfoCache,
            IMinIdInfoRepository minIdInfoRepository)
        {
            MinIdInfoCache = minIdInfoCache;
            MinIdInfoRepository = minIdInfoRepository;
        }

        /// <summary>
        /// Check bizType minIdInfo exist
        /// </summary>
        /// <param name="bizType"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistAsync([NotNull] string bizType)
        {
            Check.NotNullOrWhiteSpace(bizType, nameof(bizType));

            var minIdInfoCacheItem = await MinIdInfoCache.GetOrAddAsync(bizType, async () =>
            {
                var minIdInfo = await MinIdInfoRepository.FindByBizTypeAsync(bizType);
                return minIdInfo?.ToCacheItem();
            });

            return minIdInfoCacheItem != null;
        }

    }
}
