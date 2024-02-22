using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.CryptoVault
{
    [Authorize(CryptoVaultPermissions.RSACreds.Default)]
    public class RSACredsAppService : CryptoVaultAppServiceBase, IRSACredsAppService
    {
        protected IKeyService KeyService { get; }
        protected IRSACredsRepository RSACredsRepository { get; }
        public RSACredsAppService(
            IKeyService keyService,
            IRSACredsRepository rSACredsRepository)
        {
            KeyService = keyService;
            RSACredsRepository = rSACredsRepository;
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<RSACredsDto> GetAsync(Guid id)
        {
            var rsaCreds = await RSACredsRepository.GetAsync(id);
            return ObjectMapper.Map<RSACreds, RSACredsDto>(rsaCreds);
        }

        /// <summary>
        /// Find by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public virtual async Task<RSACredsDto> FindByIdentifierAsync([NotNull] string identifier)
        {
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            var rsaCreds = await RSACredsRepository.FindByIdentifierAsync(identifier);
            return ObjectMapper.Map<RSACreds, RSACredsDto>(rsaCreds);
        }

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="identifier"></param>
        /// <param name="sourceType"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public virtual async Task<List<RSACredsDto>> GetListAsync(string sorting = null, string identifier = "", int? sourceType = null, int? size = null)
        {
            var rsaCreds = await RSACredsRepository.GetListAsync(sorting, identifier, sourceType, size);
            return ObjectMapper.Map<List<RSACreds>, List<RSACredsDto>>(rsaCreds);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<PagedResultDto<RSACredsDto>> GetPagedListAsync(
            RSACredsPagedRequestDto input)
        {
            var count = await RSACredsRepository.GetCountAsync(input.Identifier, input.SourceType, input.Size);
            var rsaCreds = await RSACredsRepository.GetPagedListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Identifier,
                input.SourceType,
                input.Size);

            return new PagedResultDto<RSACredsDto>(
              count,
              ObjectMapper.Map<List<RSACreds>, List<RSACredsDto>>(rsaCreds)
              );
        }

        /// <summary>
        /// Generate key
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(CryptoVaultPermissions.RSACreds.Generate)]
        public virtual async Task GenerateAsync(GenerateRSACredsDto input)
        {
            var rsaCredsList = KeyService.GenerateRSACreds(input.Size, input.Count);
            await RSACredsRepository.InsertManyAsync(rsaCredsList);
        }

        /// <summary>
        /// Import key
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(CryptoVaultPermissions.RSACreds.Import)]
        public virtual async Task<RSACredsDto> ImportAsync(ImportRSACredsDto input)
        {
            var rsaCreds = new RSACreds(GuidGenerator.Create())
            {
                Identifier = KeyService.GenerateIdentifier(),
                SourceType = (int)KeySourceType.Import,
                Size = input.Size,
                PassPhrase = KeyService.GeneratePassPhrase(),
                Salt = KeyService.GenerateSalt(),
            };

            var pub = KeyService.EncryptKey(input.PublicKey, rsaCreds.PassPhrase, rsaCreds.Salt);
            var priv = KeyService.EncryptKey(input.PrivateKey, rsaCreds.PassPhrase, rsaCreds.Salt);
            rsaCreds.PublicKey = pub;
            rsaCreds.PrivateKey = priv;

            await RSACredsRepository.InsertAsync(rsaCreds);
            return ObjectMapper.Map<RSACreds, RSACredsDto>(rsaCreds);
        }


        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(CryptoVaultPermissions.RSACreds.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await RSACredsRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Decrypt Private
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(CryptoVaultPermissions.RSACreds.DecryptKey)]
        public virtual async Task<RSACredsKeyDto> GetDecryptKey(Guid id)
        {
            var rsaCreds = await RSACredsRepository.GetAsync(id);
            var publicKey = KeyService.DecryptKey(rsaCreds.PublicKey, rsaCreds.PassPhrase, rsaCreds.Salt);
            var privateKey = KeyService.DecryptKey(rsaCreds.PrivateKey, rsaCreds.PassPhrase, rsaCreds.Salt);
            return new RSACredsKeyDto()
            {
                PublicKey = publicKey,
                PrivateKey = privateKey,
            };
        }



    }
}
