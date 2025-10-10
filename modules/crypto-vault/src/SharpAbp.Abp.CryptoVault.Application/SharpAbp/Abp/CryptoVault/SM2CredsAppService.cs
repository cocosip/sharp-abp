﻿using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Application service implementation for managing SM2 cryptographic credentials.
    /// This service provides comprehensive SM2 credential management capabilities including
    /// CRUD operations, key generation, and secure key decryption based on the SM2 elliptic curve algorithm.
    /// </summary>
    /// <remarks>
    /// SM2 is a public key cryptographic algorithm based on elliptic curves, specified in the
    /// Chinese National Standard GB/T 32918. This implementation enforces authorization policies
    /// for all operations and ensures secure handling of cryptographic material. All SM2 keys
    /// are encrypted at rest using the integrated key service with configurable elliptic curves.
    /// 
    /// Authorization Requirements:
    /// - Default operations require CryptoVaultPermissions.SM2Creds.Default
    /// - Key generation requires CryptoVaultPermissions.SM2Creds.Generate
    /// - Key creation requires CryptoVaultPermissions.SM2Creds.Create
    /// - Key deletion requires CryptoVaultPermissions.SM2Creds.Delete
    /// - Key decryption requires CryptoVaultPermissions.SM2Creds.DecryptKey
    /// </remarks>
    [Authorize(CryptoVaultPermissions.SM2Creds.Default)]
    public class SM2CredsAppService : CryptoVaultAppServiceBase, ISM2CredsAppService
    {
        /// <summary>
        /// Gets the key service used for SM2 cryptographic operations.
        /// </summary>
        /// <value>
        /// The key service instance responsible for SM2 key generation, encryption, and decryption.
        /// </value>
        protected IKeyService KeyService { get; }
        
        /// <summary>
        /// Gets the SM2 credentials repository for data access operations.
        /// </summary>
        /// <value>
        /// The repository instance for accessing SM2 credentials in the data store.
        /// </value>
        protected ISM2CredsRepository SM2CredsRepository { get; }
        
        /// <summary>
        /// Initializes a new instance of the SM2CredsAppService class.
        /// </summary>
        /// <param name="keyService">The key service for SM2 cryptographic operations.</param>
        /// <param name="sM2CredsRepository">The repository for SM2 credentials data access.</param>
        /// <exception cref="ArgumentNullException">Thrown when any required service is null.</exception>
        /// <remarks>
        /// This constructor sets up the service with required dependencies for
        /// SM2 cryptographic operations and data access.
        /// </remarks>
        public SM2CredsAppService(
            IKeyService keyService,
            ISM2CredsRepository sM2CredsRepository)
        {
            KeyService = keyService;
            SM2CredsRepository = sM2CredsRepository;
        }

        /// <summary>
        /// Retrieves SM2 credentials by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the SM2 credentials to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the SM2 credentials data transfer object with encrypted keys.
        /// </returns>
        /// <exception cref="EntityNotFoundException">Thrown when SM2 credentials with the specified ID are not found.</exception>
        /// <remarks>
        /// This operation requires default read permissions for SM2 credentials.
        /// The returned data contains encrypted key information and metadata but not the decrypted keys.
        /// </remarks>
        public virtual async Task<SM2CredsDto> GetAsync(Guid id)
        {
            var sm2Creds = await SM2CredsRepository.GetAsync(id);
            return ObjectMapper.Map<SM2Creds, SM2CredsDto>(sm2Creds);
        }

        /// <summary>
        /// Finds SM2 credentials by their business identifier.
        /// </summary>
        /// <param name="identifier">The business identifier of the SM2 credentials to find. Cannot be null or whitespace.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the SM2 credentials data transfer object, or null if not found.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when the identifier is null or whitespace.</exception>
        /// <remarks>
        /// This operation performs a case-insensitive search by business identifier.
        /// Unlike GetAsync, this method returns null instead of throwing an exception when not found.
        /// Input validation ensures the identifier is not null or empty using the Check utility.
        /// </remarks>
        public virtual async Task<SM2CredsDto> FindByIdentifierAsync([NotNull] string identifier)
        {
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            var sm2Creds = await SM2CredsRepository.FindByIdentifierAsync(identifier);
            return ObjectMapper.Map<SM2Creds, SM2CredsDto>(sm2Creds);
        }

        /// <summary>
        /// Retrieves a filtered list of SM2 credentials without pagination.
        /// </summary>
        /// <param name="sorting">Optional sorting specification. If null, default sorting is applied.</param>
        /// <param name="identifier">Optional identifier filter for partial matching. Empty string means no filtering.</param>
        /// <param name="sourceType">Optional source type filter. Null means no filtering by source type.</param>
        /// <param name="curve">Optional elliptic curve filter for SM2 credentials. Empty string means no filtering.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of SM2 credentials matching the specified criteria.
        /// </returns>
        /// <remarks>
        /// This method returns all matching records without pagination limits.
        /// Filtering is applied using AND logic when multiple filters are specified.
        /// For large datasets, consider using GetPagedListAsync for better performance.
        /// The curve filter allows searching for specific SM2 elliptic curves like "sm2p256v1".
        /// </remarks>
        public virtual async Task<List<SM2CredsDto>> GetListAsync(string sorting = null, string identifier = "", int? sourceType = null, string curve = "")
        {
            var sm2Creds = await SM2CredsRepository.GetListAsync(sorting, identifier, sourceType, curve);
            return ObjectMapper.Map<List<SM2Creds>, List<SM2CredsDto>>(sm2Creds);
        }

        /// <summary>
        /// Retrieves a paginated list of SM2 credentials with filtering and sorting capabilities.
        /// </summary>
        /// <param name="input">The paged request containing pagination, sorting, and filtering parameters.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a paginated result with SM2 credentials and total count.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <remarks>
        /// This is the recommended method for retrieving SM2 credentials in user interfaces
        /// that require pagination. The operation performs two queries: one for the total count
        /// and another for the paginated data, ensuring accurate pagination information.
        /// Supports filtering by identifier, source type, and elliptic curve.
        /// </remarks>
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
        /// Generates multiple SM2 key pairs and stores them in the crypto vault.
        /// </summary>
        /// <param name="input">The generation parameters specifying elliptic curve and count.</param>
        /// <returns>A task that represents the asynchronous generation operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <exception cref="ValidationException">Thrown when input validation fails or curve is not supported.</exception>
        /// <remarks>
        /// This operation requires 'Generate' permission for SM2 credentials.
        /// The generated keys are automatically encrypted using the key service and stored securely.
        /// Each generated key pair receives a unique system-generated identifier and is marked
        /// as generated (not imported). This is an efficient batch operation for creating
        /// multiple SM2 key pairs with the same elliptic curve parameters.
        /// The curve parameter must specify a valid SM2-compatible elliptic curve.
        /// </remarks>
        [Authorize(CryptoVaultPermissions.SM2Creds.Generate)]
        public virtual async Task GenerateAsync(GenerateSM2CredsDto input)
        {
            var sm2CredsList = KeyService.GenerateSM2Creds(input.Curve, input.Count);
            await SM2CredsRepository.InsertManyAsync(sm2CredsList);
        }

        /// <summary>
        /// Creates SM2 credentials from externally provided key material.
        /// </summary>
        /// <param name="input">The creation parameters containing the SM2 key pair and curve information.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the created SM2 credentials with assigned identifier.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <exception cref="ValidationException">Thrown when input validation fails or key format is invalid.</exception>
        /// <remarks>
        /// This operation requires 'Create' permission for SM2 credentials.
        /// The provided keys are validated for SM2 format and curve compatibility, then encrypted using
        /// a generated passphrase and salt. A unique business identifier is automatically
        /// generated for the credentials. The source type is set to 'Create' to distinguish
        /// from generated keys. Use this method when importing existing SM2 key pairs.
        /// The curve must be compatible with the provided key material.
        /// </remarks>
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
        /// Permanently deletes SM2 credentials from the crypto vault.
        /// </summary>
        /// <param name="id">The unique identifier of the SM2 credentials to delete.</param>
        /// <returns>A task that represents the asynchronous deletion operation.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when SM2 credentials with the specified ID are not found.</exception>
        /// <remarks>
        /// This operation requires 'Delete' permission for SM2 credentials.
        /// WARNING: This is a permanent operation that cannot be undone. The deletion removes
        /// both the encrypted keys and all associated metadata from the data store.
        /// Consider implementing soft delete patterns for audit and recovery purposes in production.
        /// </remarks>
        [Authorize(CryptoVaultPermissions.SM2Creds.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await SM2CredsRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Retrieves and decrypts the SM2 key pair for authorized cryptographic operations.
        /// </summary>
        /// <param name="id">The unique identifier of the SM2 credentials to decrypt.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the decrypted SM2 public and private keys in plain text format.
        /// </returns>
        /// <exception cref="EntityNotFoundException">Thrown when SM2 credentials with the specified ID are not found.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller lacks decrypt key permissions.</exception>
        /// <remarks>
        /// This operation requires 'DecryptKey' permission for SM2 credentials.
        /// WARNING: This operation returns highly sensitive cryptographic material in plain text.
        /// The decrypted keys should be handled securely, not logged, cached, or persisted.
        /// Access to this operation should be strictly controlled, monitored, and audited.
        /// The keys are decrypted using the stored passphrase and salt for the credential.
        /// The returned keys are suitable for SM2 elliptic curve cryptographic operations.
        /// </remarks>
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
