using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace SharpAbp.MinId
{
    public class DefaultSegmentIdService : ISegmentIdService, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected MinIdOptions Options { get; }
        protected IUnitOfWorkManager UnitOfWorkManager { get; }
        protected IMinIdInfoRepository MinIdInfoRepository { get; }
        public DefaultSegmentIdService(
            ILogger<DefaultSegmentIdService> logger,
            IOptions<MinIdOptions> options,
            IUnitOfWorkManager unitOfWorkManager,
            IMinIdInfoRepository minIdInfoRepository)
        {
            Logger = logger;
            Options = options.Value;
            UnitOfWorkManager = unitOfWorkManager;
            MinIdInfoRepository = minIdInfoRepository;
        }

        /// <summary>
        /// Get next segmentId by bizType
        /// </summary>
        /// <param name="bizType"></param>
        /// <returns></returns>
        public virtual async Task<SegmentId> GetNextSegmentIdAsync(string bizType)
        {
            var retry = Options.ConflictRetryCount;
            var timeout = Options.TransactionTimeout;

            for (var i = 0; i < retry; i++)
            {
                SegmentId segmentId = null;

                using var unitOfWork = UnitOfWorkManager.Begin(true, true, IsolationLevel.ReadCommitted, timeout);

                try
                {

                    var minIdInfo = await MinIdInfoRepository.FindByBizTypeAsync(bizType) ?? throw new AbpException($"Can not find bizType '{bizType}'.");

                    //New id
                    var newMaxId = minIdInfo.MaxId + minIdInfo.Step;

                    Logger.LogDebug("Old maxId: '{MaxId}', New maxId: '{newMaxId}', Step: {Step}", minIdInfo.MaxId, newMaxId, minIdInfo.Step);

                    minIdInfo.UpdateMaxId(newMaxId);
                    await MinIdInfoRepository.UpdateAsync(minIdInfo);
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

                if (segmentId != null)
                {
                    return segmentId;
                }
            }

            throw new AbpException("Get next segmentId conflict.");
        }

        protected virtual SegmentId ConvertToSegmentId(MinIdInfo minIdInfo)
        {
            var loadingPercent = Options.LoadingPercent;
            var segmentId = new SegmentId()
            {
                CurrentId = (minIdInfo.MaxId - minIdInfo.Step),
                MaxId = minIdInfo.MaxId,
                Remainder = minIdInfo.Remainder < 0 ? 0 : minIdInfo.Remainder,
                Delta = minIdInfo.Delta,
                LoadingId = minIdInfo.MaxId - minIdInfo.Step + minIdInfo.Step * (loadingPercent / 100)
            };

            return segmentId;
        }

    }
}
