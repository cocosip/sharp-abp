using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace SharpAbp.MinId
{
    public class MinIdTokenManager : DomainService
    {
        protected IMinIdInfoRepository MinIdInfoRepository { get; }
        protected IMinIdTokenRepository MinIdTokenRepository { get; }

        public MinIdTokenManager(
            IMinIdInfoRepository minIdInfoRepository,
            IMinIdTokenRepository minIdTokenRepository)
        {
            MinIdInfoRepository = minIdInfoRepository;
            MinIdTokenRepository = minIdTokenRepository;
        }

        /// <summary>
        /// Create minIdToken
        /// </summary>
        /// <param name="minIdToken"></param>
        /// <returns></returns>
        public virtual async Task<MinIdToken> CreateAsync(MinIdToken minIdToken)
        {
            var minIdInfo = await MinIdInfoRepository.FindByBizTypeAsync(minIdToken.BizType);
            if (minIdInfo == null)
            {
                throw new AbpException($"Could not find bizType '{minIdToken.BizType}'.");
            }

            var queryMinIdToken = await MinIdTokenRepository.FindExpectedByTokenAsync(minIdToken.BizType, minIdToken.Token);
            if (queryMinIdToken != null)
            {
                throw new AbpException($"Duplicate token '{minIdToken.Token}' with bizType '{minIdToken.BizType}'.");
            }

            return await MinIdTokenRepository.InsertAsync(minIdToken);
        }

        /// <summary>
        /// Update minIdToken
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bizType"></param>
        /// <param name="token"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public virtual async Task<MinIdToken> UpdateAsync(Guid id, string bizType, string token, string remark)
        {
            var minIdToken = await MinIdTokenRepository.GetAsync(id);
            if (minIdToken.BizType != bizType)
            {
                //BizType changed
                var minIdInfo = await MinIdInfoRepository.FindByBizTypeAsync(bizType);
                if (minIdInfo == null)
                {
                    throw new AbpException($"Could not find bizType '{bizType}'.");
                }
            }

            var queryMinIdToken = await MinIdTokenRepository.FindExpectedByTokenAsync(bizType, token, id);
            if (queryMinIdToken != null)
            {
                throw new AbpException($"Duplicate token '{minIdToken.Token}' with bizType '{minIdToken.BizType}'.");
            }

            minIdToken.Update(bizType, token, remark);

            return await MinIdTokenRepository.UpdateAsync(minIdToken);
        }

    }
}
