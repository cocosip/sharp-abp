using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.CryptoVault
{
    public interface IRSACredsAppService : IApplicationService
    {
        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RSACredsDto> GetAsync(Guid id);

        /// <summary>
        /// Find by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        Task<RSACredsDto> FindByIdentifierAsync(string identifier);

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="identifier"></param>
        /// <param name="sourceType"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<List<RSACredsDto>> GetListAsync(string sorting = null, string identifier = "", int? sourceType = null, int? size = null);

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<RSACredsDto>> GetPagedListAsync(RSACredsPagedRequestDto input);

        /// <summary>
        /// Generate key
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task GenerateAsync(GenerateRSACredsDto input);

        /// <summary>
        /// Create key
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<RSACredsDto> CreateAsync(CreateRSACredsDto input);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Decrypt Private
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RSACredsKeyDto> GetDecryptKey(Guid id);
    }
}
