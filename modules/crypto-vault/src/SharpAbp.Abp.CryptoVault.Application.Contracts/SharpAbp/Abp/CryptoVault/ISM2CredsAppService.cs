﻿using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Application service contract for managing SM2 cryptographic credentials.
    /// This interface defines operations for creating, retrieving, and managing SM2 key pairs
    /// in the crypto vault system based on the SM2 elliptic curve cryptographic algorithm.
    /// </summary>
    /// <remarks>
    /// SM2 is a public key cryptographic algorithm based on elliptic curves, which is part of
    /// the Chinese National Standard for cryptographic algorithms. This service provides
    /// comprehensive SM2 credential management capabilities including:
    /// - CRUD operations for SM2 credentials
    /// - Key generation and import functionality with configurable curves
    /// - Secure key decryption for authorized cryptographic operations
    /// - Paginated and filtered querying capabilities
    /// All operations are subject to authorization policies defined in CryptoVaultPermissions.
    /// </remarks>
    public interface ISM2CredsAppService : IApplicationService
    {
        /// <summary>
        /// Retrieves SM2 credentials by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the SM2 credentials to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the SM2 credentials data transfer object.
        /// </returns>
        /// <remarks>
        /// This operation requires the caller to have read permissions for SM2 credentials.
        /// The returned data contains encrypted key information and metadata but not the decrypted keys.
        /// </remarks>
        Task<SM2CredsDto> GetAsync(Guid id);

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
        /// The NotNull annotation ensures compile-time null checking for the identifier parameter.
        /// </remarks>
        Task<SM2CredsDto> FindByIdentifierAsync([NotNull] string identifier);

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
        /// Use GetPagedListAsync for large datasets to improve performance.
        /// Filtering is applied using AND logic when multiple filters are specified.
        /// The curve parameter allows filtering by specific SM2 elliptic curves like "sm2p256v1".
        /// </remarks>
        Task<List<SM2CredsDto>> GetListAsync(string sorting = null, string identifier = "", int? sourceType = null, string curve = "");

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
        /// that require pagination. The response includes both the data and total count
        /// for proper pagination display. Supports filtering by identifier, source type, and curve.
        /// </remarks>
        Task<PagedResultDto<SM2CredsDto>> GetPagedListAsync(SM2CredsPagedRequestDto input);

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
        /// The provided keys are validated for SM2 format compatibility, encrypted, and stored securely.
        /// A unique business identifier is automatically generated for the credentials.
        /// Use this method when importing existing SM2 key pairs into the vault.
        /// The curve parameter must specify a valid SM2 elliptic curve.
        /// </remarks>
        Task<SM2CredsDto> CreateAsync(CreateSM2CredsDto input);

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
        /// multiple SM2 key pairs with the same curve parameters.
        /// Supported curves include "sm2p256v1" and other SM2-compatible elliptic curves.
        /// </remarks>
        Task GenerateAsync(GenerateSM2CredsDto input);

        /// <summary>
        /// Permanently deletes SM2 credentials from the crypto vault.
        /// </summary>
        /// <param name="id">The unique identifier of the SM2 credentials to delete.</param>
        /// <returns>A task that represents the asynchronous deletion operation.</returns>
        /// <remarks>
        /// This operation requires 'Delete' permission for SM2 credentials.
        /// WARNING: This is a permanent operation that cannot be undone.
        /// The deletion removes both the encrypted keys and all associated metadata.
        /// Consider implementing soft delete for audit and recovery purposes in production.
        /// </remarks>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves and decrypts the SM2 key pair for authorized cryptographic operations.
        /// </summary>
        /// <param name="id">The unique identifier of the SM2 credentials to decrypt.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the decrypted SM2 public and private keys.
        /// </returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller lacks decrypt key permissions.</exception>
        /// <remarks>
        /// This operation requires 'DecryptKey' permission for SM2 credentials.
        /// WARNING: This operation returns highly sensitive cryptographic material in plain text.
        /// The decrypted keys should be handled securely, not logged, cached, or persisted.
        /// Access to this operation should be strictly controlled, monitored, and audited.
        /// The returned keys are in their original unencrypted format suitable for SM2 operations.
        /// </remarks>
        Task<SM2CredsKeyDto> GetDecryptKey(Guid id);
    }
}
