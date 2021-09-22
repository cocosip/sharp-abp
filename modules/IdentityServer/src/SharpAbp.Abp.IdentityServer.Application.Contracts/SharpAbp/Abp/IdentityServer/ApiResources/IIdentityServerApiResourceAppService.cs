using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.IdentityServer.ApiResources
{
    public interface IIdentityServerApiResourceAppService : IApplicationService
    {
        /// <summary>
        /// Get apiResource
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ApiResourceDto> GetAsync(Guid id);

        /// <summary>
        /// Find by apiSourceName
        /// </summary>
        /// <param name="apiResourceName"></param>
        /// <returns></returns>
        Task<ApiResourceDto> FindByNameAsync(string apiResourceName);

        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        Task<List<ApiResourceDto>> GetAllAsync();

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<ApiResourceDto>> GetPagedListAsync(ApiResourcePagedRequestDto input);

        /// <summary>
        /// Get list by scopeNames
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        Task<List<ApiResourceDto>> GetListByScopesAsync(string[] scopeNames);

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Guid> CreateAsync(CreateApiResourceDto input);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id, UpdateApiResourceDto input);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);


    }
}
