using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SharpAbp.MinId
{
    public interface IMinIdGeneratorAppService : IApplicationService
    {
        /// <summary>
        /// Next id
        /// </summary>
        /// <param name="bizType"></param>
        /// <returns></returns>
        Task<long> NextIdAsync(string bizType);

        /// <summary>
        /// Batch next id
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        Task<List<long>> NextIdAsync(string bizType, int batchSize);
    }
}
