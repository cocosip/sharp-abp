using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    public interface IMinIdGenerator
    {
        /// <summary>
        /// Load current segment
        /// </summary>
        /// <returns></returns>
        Task LoadCurrentAsync();

        /// <summary>
        /// Load next segmentId
        /// </summary>
        /// <returns></returns>
        Task LoadNextAsync();

        /// <summary>
        /// NextId
        /// </summary>
        /// <returns></returns>
        Task<long> NextIdAsync();

        /// <summary>
        /// Batch nextId
        /// </summary>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        Task<List<long>> NextIdAsync(int batchSize);
    }
}
