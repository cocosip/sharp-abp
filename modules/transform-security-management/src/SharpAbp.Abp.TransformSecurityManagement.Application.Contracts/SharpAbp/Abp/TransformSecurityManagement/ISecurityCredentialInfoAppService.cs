using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    /// <summary>
    /// Application service interface for managing security credential information entities.
    /// This service provides comprehensive CRUD operations and query capabilities for security credentials
    /// within the Transform Security Management system. It serves as the primary API contract for
    /// client applications to interact with security credential data while ensuring proper authorization
    /// and business logic enforcement.
    /// </summary>
    /// <remarks>
    /// This interface defines the contract for security credential management operations including:
    /// - Retrieval of individual credentials by ID or business identifier
    /// - Listing and filtering capabilities with pagination support
    /// - Creation of new security credentials with automatic key generation
    /// - Deletion of existing credentials with proper authorization
    /// 
    /// All operations are designed to work with DTOs to ensure proper data encapsulation
    /// and API versioning support. The service integrates with the authorization system
    /// to enforce permission-based access control at the method level.
    /// </remarks>
    public interface ISecurityCredentialInfoAppService : IApplicationService
    {
        /// <summary>
        /// Retrieves a security credential information entity by its unique identifier.
        /// This method provides direct access to a specific credential using its primary key,
        /// typically used in scenarios where the exact credential ID is known.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the security credential to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the security credential information DTO if found.
        /// </returns>
        /// <remarks>
        /// This operation requires the caller to have the appropriate read permissions
        /// for security credential information. The returned DTO includes all non-sensitive
        /// metadata about the credential while ensuring that actual cryptographic material
        /// remains securely stored in the vault system.
        /// </remarks>
        Task<SecurityCredentialInfoDto> GetAsync(Guid id);

        /// <summary>
        /// Finds a security credential information entity by its business identifier.
        /// This method enables lookup using the human-readable business identifier rather than
        /// the technical GUID, supporting integration scenarios and configuration-based access.
        /// </summary>
        /// <param name="identifier">The business identifier of the security credential to find. Cannot be null or whitespace.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the security credential information DTO if found, or null if not found.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the identifier parameter is null, empty, or contains only whitespace.
        /// </exception>
        /// <remarks>
        /// This method provides a more user-friendly way to locate credentials using business identifiers
        /// that are typically used in configuration files, API calls, or administrative interfaces.
        /// Unlike GetAsync, this method returns null instead of throwing an exception when the credential is not found,
        /// making it suitable for existence checks and optional credential resolution scenarios.
        /// </remarks>
        Task<SecurityCredentialInfoDto> FindByIdentifierAsync(string identifier);

        /// <summary>
        /// Retrieves a filtered list of security credential information entities based on the specified criteria.
        /// This method supports comprehensive filtering by multiple attributes and provides flexible sorting capabilities
        /// for building custom views and reports of security credentials.
        /// </summary>
        /// <param name="sorting">Optional sorting expression to apply to the results. If null, defaults to sorting by ID.</param>
        /// <param name="identifier">Optional filter by credential business identifier. If empty, no identifier filtering is applied.</param>
        /// <param name="keyType">Optional filter by cryptographic key type (e.g., "RSA", "SM2"). If empty, no key type filtering is applied.</param>
        /// <param name="bizType">Optional filter by business type or category. If empty, no business type filtering is applied.</param>
        /// <param name="expiresMin">Optional filter for credentials expiring on or after this date. If null, no minimum expiration filter is applied.</param>
        /// <param name="expiresMax">Optional filter for credentials expiring before this date. If null, no maximum expiration filter is applied.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of security credential information DTOs matching the specified criteria.
        /// </returns>
        /// <remarks>
        /// This method provides maximum flexibility for querying credentials with multiple filter options.
        /// All filter parameters are optional and are applied only when provided with valid values.
        /// The method is suitable for scenarios where pagination is not required or when working with smaller datasets.
        /// For large datasets, consider using GetPagedListAsync for better performance and user experience.
        /// </remarks>
        Task<List<SecurityCredentialInfoDto>> GetListAsync(string sorting = null, string identifier = "", string keyType = "", string bizType = "", DateTime? expiresMin = null, DateTime? expiresMax = null);

        /// <summary>
        /// Retrieves a paginated and filtered list of security credential information entities.
        /// This method combines comprehensive filtering capabilities with pagination support,
        /// making it ideal for user interfaces that display large numbers of credentials with paging controls.
        /// </summary>
        /// <param name="input">The request DTO containing pagination parameters and filter criteria.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a paginated result with the total count and the current page of security credential DTOs.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the input parameter is null.
        /// </exception>
        /// <remarks>
        /// This method is the recommended approach for displaying security credentials in user interfaces
        /// that require pagination, such as data grids or list views. It efficiently handles large datasets
        /// by returning only the requested page of results along with the total count for pagination controls.
        /// The method supports all the same filtering capabilities as GetListAsync while providing superior
        /// performance for large result sets through server-side pagination.
        /// </remarks>
        Task<PagedResultDto<SecurityCredentialInfoDto>> GetPagedListAsync(SecurityCredentialInfoPagedRequestDto input);

        /// <summary>
        /// Creates a new security credential information entity with automatically generated cryptographic keys.
        /// This method handles the complete credential creation workflow including key generation,
        /// secure storage in the vault system, and metadata persistence.
        /// </summary>
        /// <param name="input">The creation request DTO containing the business type and other required information.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the newly created security credential information DTO with generated identifiers and metadata.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the input parameter is null.
        /// </exception>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">
        /// Thrown when the input DTO fails validation, such as missing required business type.
        /// </exception>
        /// <exception cref="Volo.Abp.Authorization.AbpAuthorizationException">
        /// Thrown when the current user lacks the required permissions to create security credentials.
        /// </exception>
        /// <remarks>
        /// This operation requires the caller to have the Create permission for security credentials.
        /// The method orchestrates the complete credential creation process:
        /// 1. Validates the input parameters
        /// 2. Generates cryptographic keys using the security credential manager
        /// 3. Stores the keys securely in the vault system
        /// 4. Persists the credential metadata in the database
        /// 5. Returns the complete credential information for immediate use
        /// 
        /// The generated credential includes automatically assigned business identifiers
        /// and vault references, ensuring uniqueness and proper integration with the security infrastructure.
        /// </remarks>
        Task<SecurityCredentialInfoDto> CreateAsync(CreateSecurityCredentialInfoDto input);

        /// <summary>
        /// Deletes a security credential information entity and its associated cryptographic material.
        /// This method permanently removes the credential from both the metadata store and the vault system,
        /// ensuring complete cleanup of all associated resources.
        /// </summary>
        /// <param name="id">The unique identifier of the security credential to delete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="Volo.Abp.Authorization.AbpAuthorizationException">
        /// Thrown when the current user lacks the required permissions to delete security credentials.
        /// </exception>
        /// <remarks>
        /// This operation requires the caller to have the Delete permission for security credentials.
        /// CAUTION: This operation is irreversible and will permanently remove the credential and its cryptographic keys.
        /// Before deletion, ensure that:
        /// 1. The credential is no longer referenced by active systems or configurations
        /// 2. Any data encrypted with this credential has been migrated or is no longer needed
        /// 3. Proper backup procedures have been followed if credential recovery might be needed
        /// 
        /// The deletion process includes cleanup of both the metadata record and the secure vault entry
        /// to prevent orphaned resources and maintain system integrity.
        /// </remarks>
        Task DeleteAsync(Guid id);
    }
}
