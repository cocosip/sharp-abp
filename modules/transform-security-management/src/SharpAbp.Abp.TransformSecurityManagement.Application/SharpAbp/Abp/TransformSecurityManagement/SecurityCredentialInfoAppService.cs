using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using SharpAbp.Abp.TransformSecurity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    /// <summary>
    /// Application service implementation for managing security credential information entities.
    /// This service provides comprehensive CRUD operations and business logic for security credentials
    /// within the Transform Security Management system, including integration with the cryptographic
    /// vault system for secure key management and storage.
    /// </summary>
    /// <remarks>
    /// This implementation handles the complete lifecycle of security credentials including:
    /// - Automated key generation and secure storage through ISecurityCredentialManager
    /// - Repository-based data access for credential metadata management
    /// - Authorization enforcement through method-level permission attributes
    /// - Object mapping between domain entities and DTOs for API responses
    /// - Comprehensive error handling and validation
    /// 
    /// The service integrates with multiple system components:
    /// - ISecurityCredentialManager: For cryptographic operations and vault integration
    /// - ISecurityCredentialInfoRepository: For data persistence and querying
    /// - ABP Authorization: For permission-based access control
    /// - AutoMapper: For entity-to-DTO mapping
    /// 
    /// All operations are designed to be thread-safe and support concurrent access patterns
    /// typical in multi-tenant web applications.
    /// </remarks>
    [Authorize(TransformSecurityManagementPermissions.SecurityCredentialInfos.Default)]
    public class SecurityCredentialInfoAppService : TransformSecurityManagementAppServiceBase, ISecurityCredentialInfoAppService
    {
        /// <summary>
        /// Gets the security credential manager responsible for cryptographic operations and vault integration.
        /// This manager handles key generation, secure storage, and retrieval of cryptographic material
        /// while maintaining separation between metadata and sensitive key data.
        /// </summary>
        protected ISecurityCredentialManager SecurityCredentialManager { get; }
        
        /// <summary>
        /// Gets the repository for security credential information entities.
        /// This repository provides data access operations for credential metadata
        /// including querying, filtering, and persistence operations.
        /// </summary>
        protected ISecurityCredentialInfoRepository SecurityCredentialInfoRepository { get; }
        
        /// <summary>
        /// Initializes a new instance of the SecurityCredentialInfoAppService class.
        /// This constructor sets up the service with required dependencies for credential management
        /// and data access operations.
        /// </summary>
        /// <param name="securityCredentialManager">The security credential manager for cryptographic operations.</param>
        /// <param name="securityCredentialInfoRepository">The repository for credential metadata operations.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when any of the required dependencies are null.
        /// </exception>
        public SecurityCredentialInfoAppService(
            ISecurityCredentialManager securityCredentialManager,
            ISecurityCredentialInfoRepository securityCredentialInfoRepository)
        {
            SecurityCredentialManager = securityCredentialManager;
            SecurityCredentialInfoRepository = securityCredentialInfoRepository;
        }

        /// <summary>
        /// Retrieves a security credential information entity by its unique identifier.
        /// This method provides direct access to credential metadata using the primary key,
        /// with automatic mapping to the appropriate DTO for API consumption.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the security credential to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the security credential information DTO.
        /// </returns>
        /// <exception cref="Volo.Abp.Domain.Entities.EntityNotFoundException">
        /// Thrown when no security credential with the specified ID is found.
        /// </exception>
        /// <remarks>
        /// This operation requires the caller to have the default read permissions for security credentials.
        /// The method retrieves the entity from the repository and maps it to a DTO,
        /// ensuring that sensitive cryptographic material is not exposed in the response.
        /// Authorization is enforced at the class level through the Authorize attribute.
        /// </remarks>
        public virtual async Task<SecurityCredentialInfoDto> GetAsync(Guid id)
        {
            var securityCredentialInfo = await SecurityCredentialInfoRepository.GetAsync(id);
            return ObjectMapper.Map<SecurityCredentialInfo, SecurityCredentialInfoDto>(securityCredentialInfo);
        }

        /// <summary>
        /// Finds a security credential information entity by its business identifier.
        /// This method enables credential lookup using human-readable business identifiers,
        /// supporting configuration-based access and integration scenarios.
        /// </summary>
        /// <param name="identifier">The business identifier of the security credential. Cannot be null or whitespace.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the security credential information DTO if found, or null if not found.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the identifier parameter is null, empty, or contains only whitespace.
        /// </exception>
        /// <remarks>
        /// This method provides parameter validation using the Check utility to ensure data integrity.
        /// Unlike GetAsync, this method returns null for missing credentials rather than throwing an exception,
        /// making it suitable for optional credential resolution scenarios.
        /// The operation maintains the same authorization requirements as other read operations.
        /// </remarks>
        public virtual async Task<SecurityCredentialInfoDto> FindByIdentifierAsync([NotNull] string identifier)
        {
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            var securityCredentialInfo = await SecurityCredentialInfoRepository.FindByIdentifierAsync(identifier);
            return ObjectMapper.Map<SecurityCredentialInfo, SecurityCredentialInfoDto>(securityCredentialInfo);
        }

        /// <summary>
        /// Retrieves a filtered list of security credential information entities.
        /// This method supports comprehensive filtering by multiple criteria and flexible sorting,
        /// enabling custom views and reports of credential data.
        /// </summary>
        /// <param name="sorting">Optional sorting expression. If null, defaults to ID-based sorting.</param>
        /// <param name="identifier">Optional business identifier filter. Empty string ignores this filter.</param>
        /// <param name="keyType">Optional cryptographic key type filter. Empty string ignores this filter.</param>
        /// <param name="bizType">Optional business type filter. Empty string ignores this filter.</param>
        /// <param name="expiresMin">Optional minimum expiration date filter. Null ignores this filter.</param>
        /// <param name="expiresMax">Optional maximum expiration date filter. Null ignores this filter.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of security credential information DTOs matching the criteria.
        /// </returns>
        /// <remarks>
        /// This method delegates filtering logic to the repository layer for optimal database performance.
        /// All filter parameters are optional and use conditional application in the repository.
        /// The method is suitable for scenarios without pagination requirements.
        /// For large datasets, consider using GetPagedListAsync for better performance.
        /// </remarks>
        public virtual async Task<List<SecurityCredentialInfoDto>> GetListAsync(
            string sorting = null,
            string identifier = "",
            string keyType = "",
            string bizType = "",
            DateTime? expiresMin = null,
            DateTime? expiresMax = null)
        {
            var securityCredentialInfos = await SecurityCredentialInfoRepository.GetListAsync(sorting, identifier, keyType, bizType, expiresMin, expiresMax);
            return ObjectMapper.Map<List<SecurityCredentialInfo>, List<SecurityCredentialInfoDto>>(securityCredentialInfos);
        }

        /// <summary>
        /// Retrieves a paginated and filtered list of security credential information entities.
        /// This method combines comprehensive filtering with efficient pagination support,
        /// optimized for user interface scenarios requiring large dataset navigation.
        /// </summary>
        /// <param name="input">The paginated request DTO containing filter criteria and pagination parameters.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a paginated result with total count and current page data.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the input parameter is null.
        /// </exception>
        /// <remarks>
        /// This method performs two optimized database operations:
        /// 1. Count query to determine total matching records for pagination metadata
        /// 2. Data query to retrieve the specific page of results
        /// 
        /// The implementation ensures consistency between count and data queries by using identical filter criteria.
        /// This approach provides accurate pagination information while maintaining good performance
        /// through server-side filtering and pagination. The method is thread-safe and supports
        /// concurrent access patterns typical in web applications.
        /// </remarks>
        public virtual async Task<PagedResultDto<SecurityCredentialInfoDto>> GetPagedListAsync(
            SecurityCredentialInfoPagedRequestDto input)
        {
            var count = await SecurityCredentialInfoRepository.GetCountAsync(
                input.Identifier,
                input.KeyType,
                input.BizType,
                input.ExpiresMin,
                input.ExpiresMax);

            var securityCredentialInfos = await SecurityCredentialInfoRepository.GetPagedListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Identifier,
                input.KeyType,
                input.BizType,
                input.ExpiresMin,
                input.ExpiresMax);

            return new PagedResultDto<SecurityCredentialInfoDto>(
              count,
              ObjectMapper.Map<List<SecurityCredentialInfo>, List<SecurityCredentialInfoDto>>(securityCredentialInfos)
              );
        }

        /// <summary>
        /// Creates a new security credential with automated key generation and secure storage.
        /// This method orchestrates the complete credential creation workflow including cryptographic
        /// key generation, vault storage, metadata persistence, and response mapping.
        /// </summary>
        /// <param name="input">The creation request DTO containing business type and configuration parameters.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the newly created security credential information DTO.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the input parameter is null.
        /// </exception>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">
        /// Thrown when the input DTO fails validation.
        /// </exception>
        /// <exception cref="Volo.Abp.Authorization.AbpAuthorizationException">
        /// Thrown when the current user lacks Create permissions.
        /// </exception>
        /// <remarks>
        /// This operation requires explicit Create permission beyond the default read permissions.
        /// The method workflow includes:
        /// 1. Input validation and authorization check
        /// 2. Cryptographic key generation via SecurityCredentialManager
        /// 3. Secure storage in the vault system
        /// 4. Metadata persistence in the database
        /// 5. Credential retrieval and DTO mapping for response
        /// 
        /// The implementation ensures atomicity of the creation process and provides
        /// comprehensive error handling for various failure scenarios. All generated
        /// identifiers and vault references are automatically assigned to ensure uniqueness
        /// and proper system integration.
        /// </remarks>
        [Authorize(TransformSecurityManagementPermissions.SecurityCredentialInfos.Create)]
        public virtual async Task<SecurityCredentialInfoDto> CreateAsync(CreateSecurityCredentialInfoDto input)
        {
            var securityCredential = await SecurityCredentialManager.GenerateAsync(input.BizType);
            var securityCredentialInfo = await SecurityCredentialInfoRepository.FindByIdentifierAsync(securityCredential.Identifier);
            return ObjectMapper.Map<SecurityCredentialInfo, SecurityCredentialInfoDto>(securityCredentialInfo);
        }

        /// <summary>
        /// Deletes a security credential and its associated cryptographic material.
        /// This method permanently removes both the metadata record and the secure vault entry,
        /// ensuring complete cleanup of all credential-related resources.
        /// </summary>
        /// <param name="id">The unique identifier of the security credential to delete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="Volo.Abp.Domain.Entities.EntityNotFoundException">
        /// Thrown when no security credential with the specified ID exists.
        /// </exception>
        /// <exception cref="Volo.Abp.Authorization.AbpAuthorizationException">
        /// Thrown when the current user lacks Delete permissions.
        /// </exception>
        /// <remarks>
        /// This operation requires explicit Delete permission beyond the default read permissions.
        /// CAUTION: This is a destructive operation that permanently removes the credential.
        /// 
        /// Before deletion, consider:
        /// - Whether the credential is referenced by active systems
        /// - If encrypted data exists that requires this credential for decryption
        /// - Whether backup/recovery procedures should be followed
        /// 
        /// The deletion process removes the metadata record from the database.
        /// Additional cleanup of vault entries may be handled through separate processes
        /// or cascade operations depending on the vault system configuration.
        /// This ensures system integrity and prevents orphaned resources.
        /// </remarks>
        [Authorize(TransformSecurityManagementPermissions.SecurityCredentialInfos.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await SecurityCredentialInfoRepository.DeleteAsync(id);
        }
    }
}
