using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using Volo.Abp;
using JetBrains.Annotations;
using Volo.Abp.ObjectMapping;

namespace SharpAbp.Abp.CryptoVault
{
    [Authorize(CryptoVaultPermissions.SM2Creds.Default)]
    public class SM2CredsAppService : CryptoVaultAppServiceBase, ISM2CredsAppService
    {
        protected IKeyService KeyService { get; }
        protected ISM2CredsRepository SM2CredsRepository { get; }
        public SM2CredsAppService(
            IKeyService keyService, 
            ISM2CredsRepository sM2CredsRepository)
        {
            KeyService = keyService;
            SM2CredsRepository = sM2CredsRepository;
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<SM2CredsDto> GetAsync(Guid id)
        {
            var sm2Creds = await SM2CredsRepository.GetAsync(id);
            return ObjectMapper.Map<SM2Creds, SM2CredsDto>(sm2Creds);
        }

        /// <summary>
        /// Find by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public virtual async Task<SM2CredsDto> FindByIdentifierAsync([NotNull] string identifier)
        {
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            var sm2Creds = await SM2CredsRepository.FindByIdentifierAsync(identifier);
            return ObjectMapper.Map<SM2Creds, SM2CredsDto>(sm2Creds);
        }

    }
}
