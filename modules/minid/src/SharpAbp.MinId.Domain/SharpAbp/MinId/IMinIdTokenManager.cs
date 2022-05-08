using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace SharpAbp.MinId
{
    public interface IMinIdTokenManager : IDomainService
    {
        /// <summary>
        /// Create minIdToken
        /// </summary>
        /// <param name="minIdToken"></param>
        /// <returns></returns>
        Task CreateAsync(MinIdToken minIdToken);

        /// <summary>
        /// Update minIdToken
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bizType"></param>
        /// <param name="token"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id, string bizType, string token, string remark);
    }
}
