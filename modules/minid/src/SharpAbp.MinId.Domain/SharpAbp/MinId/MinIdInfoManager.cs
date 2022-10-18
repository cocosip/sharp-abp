using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace SharpAbp.MinId
{
    public class MinIdInfoManager : DomainService
    {
        protected IMinIdInfoRepository MinIdInfoRepository { get; }
        public MinIdInfoManager(IMinIdInfoRepository minIdInfoRepository)
        {
            MinIdInfoRepository = minIdInfoRepository;
        }

        /// <summary>
        /// Create minIdInfo
        /// </summary>
        /// <param name="minIdInfo"></param>
        /// <returns></returns>
        public virtual async Task<MinIdInfo> CreateAsync(MinIdInfo minIdInfo)
        {
            var queryMinIdInfo = await MinIdInfoRepository.FindExpectedByBizTypeAsync(minIdInfo.BizType);
            if (queryMinIdInfo != null)
            {
                throw new AbpException($"Duplicate bizType '{minIdInfo.BizType}'.");
            }

            return await MinIdInfoRepository.InsertAsync(minIdInfo);
        }

        /// <summary>
        /// Update minIdInfo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bizType"></param>
        /// <param name="maxId"></param>
        /// <param name="step"></param>
        /// <param name="delta"></param>
        /// <param name="remainder"></param>
        /// <returns></returns>
        public virtual async Task<MinIdInfo> UpdateAsync(Guid id, string bizType, long maxId, int step, int delta, int remainder)
        {
            var minIdInfo = await MinIdInfoRepository.GetAsync(id);

            var queryMinIdInfo = await MinIdInfoRepository.FindExpectedByBizTypeAsync(minIdInfo.BizType, id);
            if (queryMinIdInfo != null)
            {
                throw new AbpException($"Duplicate bizType '{minIdInfo.BizType}'.");
            }

            minIdInfo.Update(bizType, maxId, step, delta, remainder);

            return await MinIdInfoRepository.UpdateAsync(minIdInfo);
        }

    }
}
