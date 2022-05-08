using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    public interface IMinIdGeneratorFactory
    {
        /// <summary>
        /// Get minIdGenerator
        /// </summary>
        /// <param name="bizType"></param>
        /// <returns></returns>
        Task<IMinIdGenerator> GetAsync(string bizType);
    }
}
