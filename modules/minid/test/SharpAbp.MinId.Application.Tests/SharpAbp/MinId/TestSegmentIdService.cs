using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
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


        public override async Task<SegmentId> GetNextSegmentIdAsync(string bizType, CancellationToken cancellationToken = default)
        {
            SegmentId segmentId = null;

            using var unitOfWork = UnitOfWorkManager.Begin(true, false, IsolationLevel.ReadCommitted, 1000);
            try
            {
                var minIdInfo = await MinIdInfoRepository.FindByBizTypeAsync(bizType) ?? throw new AbpException($"Can not find bizType '{bizType}'.");

                //New id
                var newMaxId = minIdInfo.MaxId + minIdInfo.Step;

                Logger.LogDebug("Old maxId:{MaxId}, New maxId {newMaxId},Step:{Step}", minIdInfo.MaxId, newMaxId, minIdInfo.Step);

                minIdInfo.UpdateMaxId(newMaxId);
                await MinIdInfoRepository.UpdateAsync(minIdInfo, cancellationToken: cancellationToken);
                segmentId = ConvertToSegmentId(minIdInfo);
            }
            catch
            {

                try
                {
                    await unitOfWork.RollbackAsync();
                }
                catch
                {
                    /* ignored */
                }

                throw;
            }
            await unitOfWork.CompleteAsync();
            return segmentId;
        }

    }
}
