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
    /// Application service implementation for managing RSA cryptographic credentials.
    /// This service provides comprehensive RSA credential management capabilities including
    /// CRUD operations, key generation, and secure key decryption.
    /// </summary>
    /// <remarks>
    /// This implementation enforces authorization policies for all operations and ensures
    /// secure handling of cryptographic material. All RSA keys are encrypted at rest
    /// using the integrated key service. The service supports both generated and imported
    /// RSA key pairs with configurable key sizes.
    /// 
    /// Authorization Requirements:
    /// - Default operations require CryptoVaultPermissions.RSACreds.Default
    /// - Key generation requires CryptoVaultPermissions.RSACreds.Generate
    /// - Key creation requires CryptoVaultPermissions.RSACreds.Create
    /// - Key deletion requires CryptoVaultPermissions.RSACreds.Delete
    /// - Key decryption requires CryptoVaultPermissions.RSACreds.DecryptKey
    /// </remarks>
    [Authorize(CryptoVaultPermissions.RSACreds.Default)]
    public class RSACredsAppService : CryptoVaultAppServiceBase, IRSACredsAppService
    {
        /// <summary>
        /// Gets the key service used for cryptographic operations.
        /// </summary>
        /// <value>
        /// The key service instance responsible for key generation, encryption, and decryption.
        /// </value>
        protected IKeyService KeyService { get; }
        
        /// <summary>
        /// Gets the RSA credentials repository for data access operations.
        /// </summary>
        /// <value>
        /// The repository instance for accessing RSA credentials in the data store.
        /// </value>
        protected IRSACredsRepository RSACredsRepository { get; }
        
        /// <summary>
        /// Initializes a new instance of the RSACredsAppService class.
        /// </summary>
        /// <param name="keyService">The key service for cryptographic operations.</param>
        /// <param name="rSACredsRepository">The repository for RSA credentials data access.</param>
        /// <exception cref="ArgumentNullException">Thrown when any required service is null.</exception>
        /// <remarks>
        /// This constructor sets up the service with required dependencies for
        /// cryptographic operations and data access.
        /// </remarks>
        public RSACredsAppService(
            IKeyService keyService,
            IRSACredsRepository rSACredsRepository)
        {
            KeyService = keyService;
            RSACredsRepository = rSACredsRepository;
        }

        /// <summary>
        /// Retrieves RSA credentials by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the RSA credentials to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the RSA credentials data transfer object with encrypted keys.
        /// </returns>
        /// <remarks>
        /// This operation requires default read permissions for RSA credentials.
        /// The returned data contains encrypted key information and metadata but not the decrypted keys.
        /// </remarks>
        public virtual async Task<RSACredsDto> GetAsync(Guid id)
        {
            var rsaCreds = await RSACredsRepository.GetAsync(id);
            return ObjectMapper.Map<RSACreds, RSACredsDto>(rsaCreds);
        }

        /// <summary>
        /// Finds RSA credentials by their business identifier.
        /// </summary>
        /// <param name="identifier">The business identifier of the RSA credentials to find.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the RSA credentials data transfer object, or null if not found.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when the identifier is null or whitespace.</exception>
        /// <remarks>
        /// This operation performs a case-insensitive search by business identifier.
        /// Unlike GetAsync, this method returns null instead of throwing an exception when not found.
        /// Input validation ensures the identifier is not null or empty.
        /// </remarks>
        public virtual async Task<RSACredsDto> FindByIdentifierAsync([NotNull] string identifier)
        {
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            var rsaCreds = await RSACredsRepository.FindByIdentifierAsync(identifier);
            return ObjectMapper.Map<RSACreds, RSACredsDto>(rsaCreds);
        }

        /// <summary>
        /// Retrieves a filtered list of RSA credentials without pagination.
        /// </summary>
        /// <param name="sorting">Optional sorting specification. If null, default sorting is applied.</param>
        /// <param name="identifier">Optional identifier filter for partial matching. Empty string means no filtering.</param>
        /// <param name="sourceType">Optional source type filter. Null means no filtering by source type.</param>
        /// <param name="size">Optional RSA key size filter. Null means no filtering by key size.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of RSA credentials matching the specified criteria.
        /// </returns>
        /// <remarks>
        /// This method returns all matching records without pagination limits.
        /// Filtering is applied using AND logic when multiple filters are specified.
        /// For large datasets, consider using GetPagedListAsync for better performance.
        /// </remarks>
        public virtual async Task<List<RSACredsDto>> GetListAsync(string sorting = null, string identifier = "", int? sourceType = null, int? size = null)
        {
            var rsaCreds = await RSACredsRepository.GetListAsync(sorting, identifier, sourceType, size);
            return ObjectMapper.Map<List<RSACreds>, List<RSACredsDto>>(rsaCreds);
        }

        /// <summary>
        /// Retrieves a paginated list of RSA credentials with filtering and sorting capabilities.
        /// </summary>
        /// <param name="input">The paged request containing pagination, sorting, and filtering parameters.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a paginated result with RSA credentials and total count.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <remarks>
        /// This is the recommended method for retrieving RSA credentials in user interfaces
        /// that require pagination. The operation performs two queries: one for the total count
        /// and another for the paginated data, ensuring accurate pagination information.
        /// </remarks>
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
        /// Generates multiple RSA key pairs and stores them in the crypto vault.
        /// </summary>
        /// <param name="input">The generation parameters specifying key size and count.</param>
        /// <returns>A task that represents the asynchronous generation operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <exception cref="ValidationException">Thrown when input validation fails.</exception>
        /// <remarks>
        /// This operation requires 'Generate' permission for RSA credentials.
        /// The generated keys are automatically encrypted using the key service and stored securely.
        /// Each generated key pair receives a unique system-generated identifier and is marked
        /// as generated (not imported). This is an efficient batch operation for creating
        /// multiple key pairs with the same parameters.
        /// </remarks>
        [Authorize(CryptoVaultPermissions.RSACreds.Generate)]
        public virtual async Task GenerateAsync(GenerateRSACredsDto input)
        {
            var rsaCredsList = KeyService.GenerateRSACreds(input.Size, input.Count);
            await RSACredsRepository.InsertManyAsync(rsaCredsList);
        }

        /// <summary>
        /// Creates RSA credentials from externally provided key material.
        /// </summary>
        /// <param name="input">The creation parameters containing the RSA key pair and metadata.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the created RSA credentials with assigned identifier.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <exception cref="ValidationException">Thrown when input validation fails or key format is invalid.</exception>
        /// <remarks>
        /// This operation requires 'Create' permission for RSA credentials.
        /// The provided keys are validated for format and compatibility, then encrypted using
        /// a generated passphrase and salt. A unique business identifier is automatically
        /// generated for the credentials. The source type is set to 'Create' to distinguish
        /// from generated keys. Use this method when importing existing RSA key pairs.
        /// </remarks>
        [Authorize(CryptoVaultPermissions.RSACreds.Create)]
        public virtual async Task<RSACredsDto> CreateAsync(CreateRSACredsDto input)
        {
            var rsaCreds = new RSACreds(GuidGenerator.Create())
            {
                Identifier = KeyService.GenerateIdentifier(),
                SourceType = (int)KeySourceType.Create,
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
        /// Permanently deletes RSA credentials from the crypto vault.
        /// </summary>
        /// <param name="id">The unique identifier of the RSA credentials to delete.</param>
        /// <returns>A task that represents the asynchronous deletion operation.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when RSA credentials with the specified ID are not found.</exception>
        /// <remarks>
        /// This operation requires 'Delete' permission for RSA credentials.
        /// WARNING: This is a permanent operation that cannot be undone. The deletion removes
        /// both the encrypted keys and all associated metadata from the data store.
        /// Consider implementing soft delete patterns for audit and recovery purposes in production.
        /// </remarks>
        [Authorize(CryptoVaultPermissions.RSACreds.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await RSACredsRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Retrieves and decrypts the RSA key pair for authorized cryptographic operations.
        /// </summary>
        /// <param name="id">The unique identifier of the RSA credentials to decrypt.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the decrypted RSA public and private keys in plain text format.
        /// </returns>
        /// <exception cref="EntityNotFoundException">Thrown when RSA credentials with the specified ID are not found.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller lacks decrypt key permissions.</exception>
        /// <remarks>
        /// This operation requires 'DecryptKey' permission for RSA credentials.
        /// WARNING: This operation returns highly sensitive cryptographic material in plain text.
        /// The decrypted keys should be handled securely, not logged, cached, or persisted.
        /// Access to this operation should be strictly controlled, monitored, and audited.
        /// The keys are decrypted using the stored passphrase and salt for the credential.
        /// </remarks>
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
