using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.MinId
{
    public interface IMinIdTokenAppService : IApplicationService
    {
        /// <summary>
        /// Get minIdToken by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MinIdTokenDto> GetAsync(Guid id);

        /// <summary>
        /// Find minIdToken by token
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<MinIdTokenDto> FindByTokenAsync(string bizType, string token);

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<MinIdTokenDto>> GetPagedListAsync(MinIdTokenPagedRequestDto input);

        /// <summary>
        /// Create minIdToken
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MinIdTokenDto> CreateAsync(CreateMinIdTokenDto input);

        /// <summary>
        /// Update minIdToken
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MinIdTokenDto> UpdateAsync(Guid id, UpdateMinIdTokenDto input);

        /// <summary>
        /// Delete minIdToken
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);
    }
}
