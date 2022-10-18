using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.MinId
{
    public interface IMinIdInfoAppService : IApplicationService
    {

        /// <summary>
        /// Get minIdInfo by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MinIdInfoDto> GetAsync(Guid id);

        /// <summary>
        /// Find minIdInfo by bizType
        /// </summary>
        /// <param name="bizType"></param>
        /// <returns></returns>
        Task<MinIdInfoDto> FindByBizTypeAsync(string bizType);

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<MinIdInfoDto>> GetPagedListAsync(MinIdInfoPagedRequestDto input);

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="bizType"></param>
        /// <returns></returns>
        Task<List<MinIdInfoDto>> GetListAsync(string sorting = null, string bizType = "");

        /// <summary>
        /// Create minIdInfo
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MinIdInfoDto> CreateAsync(CreateMinIdInfoDto input);

        /// <summary>
        /// Update minIdInfo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MinIdInfoDto> UpdateAsync(Guid id, UpdateMinIdInfoDto input);

        /// <summary>
        /// Delete minIdInfo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);
    }
}
