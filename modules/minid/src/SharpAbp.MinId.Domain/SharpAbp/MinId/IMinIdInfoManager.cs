using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace SharpAbp.MinId
{
    public interface IMinIdInfoManager : IDomainService
    {
        /// <summary>
        /// Create minIdInfo
        /// </summary>
        /// <param name="minIdInfo"></param>
        /// <returns></returns>
        Task CreateAsync(MinIdInfo minIdInfo);

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
        Task UpdateAsync(Guid id, string bizType, long maxId, int step, int delta, int remainder);
    }
}
