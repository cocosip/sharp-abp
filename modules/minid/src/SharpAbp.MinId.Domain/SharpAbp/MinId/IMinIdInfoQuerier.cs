using JetBrains.Annotations;
using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    public interface IMinIdInfoQuerier
    {
        /// <summary>
        /// Check bizType minIdInfo exist
        /// </summary>
        /// <param name="bizType"></param>
        /// <returns></returns>
        Task<bool> ExistAsync([NotNull] string bizType);
    }
}
