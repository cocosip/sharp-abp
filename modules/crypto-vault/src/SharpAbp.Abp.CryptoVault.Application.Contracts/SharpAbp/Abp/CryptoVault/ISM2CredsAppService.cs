using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.CryptoVault
{
    public interface ISM2CredsAppService : IApplicationService
    {
        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SM2CredsDto> GetAsync(Guid id);

        /// <summary>
        /// Find by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        Task<SM2CredsDto> FindByIdentifierAsync([NotNull] string identifier);

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="identifier"></param>
        /// <param name="sourceType"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        Task<List<SM2CredsDto>> GetListAsync(string sorting = null, string identifier = "", int? sourceType = null, string curve = "");


        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<SM2CredsDto>> GetPagedListAsync(SM2CredsPagedRequestDto input);

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SM2CredsDto> CreateAsync(CreateSM2CredsDto input);

        /// <summary>
        /// Generate
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task GenerateAsync(GenerateSM2CredsDto input);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Get decrypt key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SM2CredsKeyDto> GetDecryptKey(Guid id);
    }
}
