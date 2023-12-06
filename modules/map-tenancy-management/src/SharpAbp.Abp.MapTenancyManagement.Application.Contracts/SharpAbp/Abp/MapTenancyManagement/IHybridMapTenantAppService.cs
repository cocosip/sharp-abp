using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public interface IHybridMapTenantAppService : IApplicationService
    {

        /// <summary>
        /// Get HybridMapTenant by tenant id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<HybridMapTenantDto> GetAsync(Guid id);

        /// <summary>
        /// Get all HybridMapTenant
        /// </summary>
        /// <returns></returns>
        Task<List<HybridMapTenantDto>> GetAllAsync();

        /// <summary>
        /// Get paged HybridMapTenant
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<HybridMapTenantDto>> GetListAsync(MapTenantPagedRequestDto input);

        /// <summary>
        /// Search HybridMapTenant paged
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<HybridMapTenantDto>> SearchAsync(MapTenantPagedRequestDto input);

        /// <summary>
        /// Current HybridMapTenant
        /// </summary>
        /// <returns></returns>
        Task<HybridMapTenantDto> CurrentAsync();

        /// <summary>
        /// Create HybridMapTenant
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<HybridMapTenantDto> CreateAsync(CreateHybridMapTenantDto input);

        /// <summary>
        /// Update HybridMapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<HybridMapTenantDto> UpdateAsync(Guid id, UpdateHybridMapTenantDto input);

        /// <summary>
        /// Delete HybridMapTenant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);


        /// <summary>
        /// Get default connection string
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<string> GetDefaultConnectionStringAsync(Guid id);

        /// <summary>
        /// Update default connection string
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultConnectionString"></param>
        /// <returns></returns>
        Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString);

        /// <summary>
        /// Delete default connection string
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteDefaultConnectionStringAsync(Guid id);

    }
}