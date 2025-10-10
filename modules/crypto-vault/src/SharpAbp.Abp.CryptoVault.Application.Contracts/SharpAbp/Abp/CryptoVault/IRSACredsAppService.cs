﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.CryptoVault
{
    /// <summary>
    /// Application service contract for managing RSA cryptographic credentials.
    /// This interface defines operations for creating, retrieving, and managing RSA key pairs
    /// in the crypto vault system.
    /// </summary>
    /// <remarks>
    /// This service provides comprehensive RSA credential management capabilities including:
    /// - CRUD operations for RSA credentials
    /// - Key generation and import functionality
    /// - Secure key decryption for authorized users
    /// - Paginated and filtered querying capabilities
    /// All operations are subject to authorization policies defined in CryptoVaultPermissions.
    /// </remarks>
    public interface IRSACredsAppService : IApplicationService
    {
        /// <summary>
        /// Retrieves RSA credentials by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the RSA credentials to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the RSA credentials data transfer object.
        /// </returns>
        /// <remarks>
        /// This operation requires the caller to have read permissions for RSA credentials.
        /// The returned data contains encrypted key information and metadata.
        /// </remarks>
        Task<RSACredsDto> GetAsync(Guid id);

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
        /// </remarks>
        Task<RSACredsDto> FindByIdentifierAsync(string identifier);

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
        /// Use GetPagedListAsync for large datasets to improve performance.
        /// Filtering is applied using AND logic when multiple filters are specified.
        /// </remarks>
        Task<List<RSACredsDto>> GetListAsync(string sorting = null, string identifier = "", int? sourceType = null, int? size = null);

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
        /// that require pagination. The response includes both the data and total count
        /// for proper pagination display.
        /// </remarks>
        Task<PagedResultDto<RSACredsDto>> GetPagedListAsync(RSACredsPagedRequestDto input);

        /// <summary>
        /// Generates multiple RSA key pairs and stores them in the crypto vault.
        /// </summary>
        /// <param name="input">The generation parameters specifying key size and count.</param>
        /// <returns>A task that represents the asynchronous generation operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <exception cref="ValidationException">Thrown when input validation fails.</exception>
        /// <remarks>
        /// This operation requires 'Generate' permission for RSA credentials.
        /// The generated keys are automatically encrypted and stored securely.
        /// Each generated key pair receives a unique system-generated identifier.
        /// This is a batch operation that can generate multiple key pairs efficiently.
        /// </remarks>
        Task GenerateAsync(GenerateRSACredsDto input);

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
        /// The provided keys are validated, encrypted, and stored securely.
        /// A unique business identifier is automatically generated for the credentials.
        /// Use this method when importing existing RSA key pairs into the vault.
        /// </remarks>
        Task<RSACredsDto> CreateAsync(CreateRSACredsDto input);

        /// <summary>
        /// Permanently deletes RSA credentials from the crypto vault.
        /// </summary>
        /// <param name="id">The unique identifier of the RSA credentials to delete.</param>
        /// <returns>A task that represents the asynchronous deletion operation.</returns>
        /// <remarks>
        /// This operation requires 'Delete' permission for RSA credentials.
        /// WARNING: This is a permanent operation that cannot be undone.
        /// The deletion removes both the encrypted keys and all associated metadata.
        /// Consider implementing soft delete for audit and recovery purposes in production.
        /// </remarks>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves and decrypts the RSA key pair for authorized cryptographic operations.
        /// </summary>
        /// <param name="id">The unique identifier of the RSA credentials to decrypt.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the decrypted RSA public and private keys.
        /// </returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller lacks decrypt key permissions.</exception>
        /// <remarks>
        /// This operation requires 'DecryptKey' permission for RSA credentials.
        /// WARNING: This operation returns sensitive cryptographic material.
        /// The decrypted keys should be handled securely and not logged or cached.
        /// Access to this operation should be strictly controlled and audited.
        /// The returned keys are in their original unencrypted format.
        /// </remarks>
        Task<RSACredsKeyDto> GetDecryptKey(Guid id);
    }
}
