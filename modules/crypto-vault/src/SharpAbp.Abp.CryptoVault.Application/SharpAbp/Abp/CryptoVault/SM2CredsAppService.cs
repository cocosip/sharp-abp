using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

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

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="identifier"></param>
        /// <param name="sourceType"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        public virtual async Task<List<SM2CredsDto>> GetListAsync(string sorting = null, string identifier = "", int? sourceType = null, string curve = "")
        {
            var sm2Creds = await SM2CredsRepository.GetListAsync(sorting, identifier, sourceType, curve);
            return ObjectMapper.Map<List<SM2Creds>, List<SM2CredsDto>>(sm2Creds);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<PagedResultDto<SM2CredsDto>> GetPagedListAsync(SM2CredsPagedRequestDto input)
        {
            var count = await SM2CredsRepository.GetCountAsync(input.Identifier, input.SourceType, input.Curve);
            var sm2Creds = await SM2CredsRepository.GetPagedListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Identifier,
                input.SourceType,
                input.Curve);

            return new PagedResultDto<SM2CredsDto>(
              count,
              ObjectMapper.Map<List<SM2Creds>, List<SM2CredsDto>>(sm2Creds)
              );
        }

        /// <summary>
        /// Generate
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(CryptoVaultPermissions.SM2Creds.Generate)]
        public virtual async Task GenerateAsync(GenerateSM2CredsDto input)
        {
            var sm2CredsList = KeyService.GenerateSM2Creds(input.Curve, input.Count);
            await SM2CredsRepository.InsertManyAsync(sm2CredsList);
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(CryptoVaultPermissions.SM2Creds.Create)]
        public virtual async Task<SM2CredsDto> CreateAsync(CreateSM2CredsDto input)
        {
            var sm2Creds = new SM2Creds(GuidGenerator.Create())
            {
                Identifier = KeyService.GenerateIdentifier(),
                SourceType = (int)KeySourceType.Create,
                Curve = input.Curve,
                PassPhrase = KeyService.GeneratePassPhrase(),
                Salt = KeyService.GenerateSalt(),
            };

            sm2Creds.PublicKey = KeyService.EncryptKey(input.PublicKey, sm2Creds.PassPhrase, sm2Creds.Salt);
            sm2Creds.PrivateKey = KeyService.EncryptKey(input.PrivateKey, sm2Creds.PassPhrase, sm2Creds.Salt);

            await SM2CredsRepository.InsertAsync(sm2Creds);
            return ObjectMapper.Map<SM2Creds, SM2CredsDto>(sm2Creds);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(CryptoVaultPermissions.SM2Creds.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await SM2CredsRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Get decrypt key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(CryptoVaultPermissions.SM2Creds.DecryptKey)]
        public virtual async Task<SM2CredsKeyDto> GetDecryptKey(Guid id)
        {
            var sm2Creds = await SM2CredsRepository.GetAsync(id);
            var publicKey = KeyService.DecryptKey(sm2Creds.PublicKey, sm2Creds.PassPhrase, sm2Creds.Salt);
            var privateKey = KeyService.DecryptKey(sm2Creds.PrivateKey, sm2Creds.PassPhrase, sm2Creds.Salt);
            return new SM2CredsKeyDto()
            {
                PublicKey = publicKey,
                PrivateKey = privateKey,
            };
        }

    }
}
