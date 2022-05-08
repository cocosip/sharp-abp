using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Uow;

namespace SharpAbp.MinId
{
    public class TestSegmentIdService : DefaultSegmentIdService
    {
        public TestSegmentIdService(
            ILogger<DefaultSegmentIdService> logger,
            IOptions<MinIdOptions> options,
            IUnitOfWorkManager unitOfWorkManager,
            IMinIdInfoRepository minIdInfoRepository)
            : base(
                logger,
                options,
                unitOfWorkManager,
                minIdInfoRepository)
        {

        }


        public override async Task<SegmentId> GetNextSegmentIdAsync(string bizType)
        {
            SegmentId segmentId = null;
            try
            {

                using var uow = UnitOfWorkManager.Begin(true, false, IsolationLevel.ReadCommitted, 1000);

                var minIdInfo = await MinIdInfoRepository.FindByBizTypeAsync(bizType);
                if (minIdInfo == null)
                {
                    throw new AbpException($"Can not find bizType '{bizType}'.");
                }

                //New id
                var newMaxId = minIdInfo.MaxId + minIdInfo.Step;

                Logger.LogDebug("Old maxId:{0}, New maxId {1},Step:{2}", minIdInfo.MaxId, newMaxId, minIdInfo.Step);

                minIdInfo.UpdateMaxId(newMaxId);
                await MinIdInfoRepository.UpdateAsync(minIdInfo);
                await uow.SaveChangesAsync();
                segmentId = ConvertToSegmentId(minIdInfo);
            }
            catch (AbpDbConcurrencyException ex)
            {
                Logger.LogError(ex, "Get next segmentId conflict. {0}", ex.Message);
                segmentId = null;
            }
            return segmentId;
        }

    }
}
