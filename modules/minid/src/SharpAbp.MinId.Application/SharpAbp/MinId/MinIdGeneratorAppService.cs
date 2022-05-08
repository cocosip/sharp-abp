using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    public class MinIdGeneratorAppService : MinIdAppService, IMinIdGeneratorAppService
    {
        protected IMinIdGeneratorFactory MinIdGeneratorFactory { get; }
        public MinIdGeneratorAppService(IMinIdGeneratorFactory minIdGeneratorFactory)
        {
            MinIdGeneratorFactory = minIdGeneratorFactory;
        }

        /// <summary>
        /// Next id
        /// </summary>
        /// <param name="bizType"></param>
        /// <returns></returns>
        public virtual async Task<long> NextIdAsync(string bizType)
        {
            var minIdGenerator = await MinIdGeneratorFactory.GetAsync(bizType);
            return await minIdGenerator.NextIdAsync();
        }

        /// <summary>
        /// Batch next id
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public virtual async Task<List<long>> NextIdAsync(string bizType, int batchSize)
        {
            batchSize = batchSize < 1 ? 1 : batchSize;
            var minIdGenerator = await MinIdGeneratorFactory.GetAsync(bizType);
            return await minIdGenerator.NextIdAsync(batchSize);
        }
    }
}
