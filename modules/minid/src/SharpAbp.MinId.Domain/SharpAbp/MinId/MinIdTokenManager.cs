﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Domain service for managing MinId authentication tokens with business logic validation.
    /// This service implements domain-level operations for creating and updating MinId tokens
    /// while enforcing business rules such as token uniqueness within business type contexts
    /// and ensuring associated business types exist in the system.
    /// </summary>
    public class MinIdTokenManager : DomainService, IMinIdTokenManager
    {
        /// <summary>
        /// Gets the repository for MinId information persistence operations.
        /// </summary>
        protected IMinIdInfoRepository MinIdInfoRepository { get; }
        
        /// <summary>
        /// Gets the repository for MinId token persistence operations.
        /// </summary>
        protected IMinIdTokenRepository MinIdTokenRepository { get; }
        
        /// <summary>
        /// Initializes a new instance of the MinIdTokenManager with required dependencies.
        /// </summary>
        /// <param name="minIdInfoRepository">Repository for MinId information operations.</param>
        /// <param name="minIdTokenRepository">Repository for MinId token operations.</param>
        public MinIdTokenManager(
            IMinIdInfoRepository minIdInfoRepository,
            IMinIdTokenRepository minIdTokenRepository)
        {
            MinIdInfoRepository = minIdInfoRepository;
            MinIdTokenRepository = minIdTokenRepository;
        }

        /// <summary>
        /// Creates a new MinId authentication token with validation to ensure token uniqueness
        /// within the specified business type context. This method also validates that the
        /// associated business type exists in the system before creating the token.
        /// </summary>
        /// <param name="minIdToken">The MinId token entity to create. Cannot be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task containing the created MinId token entity.</returns>
        /// <exception cref="AbpException">
        /// Thrown when:
        /// - The associated business type is not found in the system
        /// - A token with the same value already exists for the specified business type
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when minIdToken is null.</exception>
        public virtual async Task<MinIdToken> CreateAsync(MinIdToken minIdToken, CancellationToken cancellationToken = default)
        {
            if (minIdToken == null)
            {
                throw new ArgumentNullException(nameof(minIdToken), "MinId token cannot be null when creating a new token.");
            }

            Logger.LogDebug("Attempting to create MinId token for business type '{BizType}' with token '{Token}'", 
                minIdToken.BizType, minIdToken.Token);

            // Validate that the business type exists in the system
            var minIdInfo = await MinIdInfoRepository.FindByBizTypeAsync(minIdToken.BizType, cancellationToken);
            if (minIdInfo == null)
            {
                var errorMessage = $"Cannot create MinId token: Business type '{minIdToken.BizType}' is not registered in the system. Please ensure the business type configuration exists before creating tokens.";
                Logger.LogWarning("Failed to create MinId token due to missing business type: {BizType}", minIdToken.BizType);
                throw new AbpException(errorMessage);
            }

            // Check for token uniqueness within the business type context
            var queryMinIdToken = await MinIdTokenRepository.FindExpectedByTokenAsync(minIdToken.BizType, minIdToken.Token, null, cancellationToken);
            if (queryMinIdToken != null)
            {
                var errorMessage = $"Cannot create MinId token: Token '{minIdToken.Token}' already exists for business type '{minIdToken.BizType}'. Each token must be unique within its business type context.";
                Logger.LogWarning("Failed to create MinId token due to duplicate token: {Token} for business type {BizType}", 
                    minIdToken.Token, minIdToken.BizType);
                throw new AbpException(errorMessage);
            }

            var result = await MinIdTokenRepository.InsertAsync(minIdToken, cancellationToken: cancellationToken);
            Logger.LogInformation("Successfully created MinId token '{Token}' for business type '{BizType}' with ID {Id}", 
                minIdToken.Token, minIdToken.BizType, result.Id);
            
            return result;
        }

        /// <summary>
        /// Updates an existing MinId authentication token with new values.
        /// This method validates that the updated token doesn't conflict with existing tokens
        /// and ensures the associated business type exists if it has been changed.
        /// </summary>
        /// <param name="id">The unique identifier of the token to update.</param>
        /// <param name="bizType">The business type identifier for the token.</param>
        /// <param name="token">The authentication token string value.</param>
        /// <param name="remark">An optional remark or description for the token.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task containing the updated MinId token entity.</returns>
        /// <exception cref="AbpException">
        /// Thrown when:
        /// - The specified token ID is not found
        /// - The business type is changed but the new business type is not found in the system
        /// - A token with the same value already exists for the specified business type
        /// </exception>
        public virtual async Task<MinIdToken> UpdateAsync(Guid id, string bizType, string token, string remark, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("Attempting to update MinId token with ID {Id} to business type '{BizType}' and token '{Token}'", 
                id, bizType, token);

            var minIdToken = await MinIdTokenRepository.GetAsync(id, cancellationToken: cancellationToken);
            if (minIdToken == null)
            {
                var errorMessage = $"Cannot update MinId token: No token found with ID '{id}'. Please verify the ID is correct.";
                Logger.LogWarning("Failed to update MinId token: ID {Id} not found", id);
                throw new AbpException(errorMessage);
            }

            var oldBizType = minIdToken.BizType;
            var oldToken = minIdToken.Token;

            // Check if business type is changing and validate the new business type exists
            if (minIdToken.BizType != bizType)
            {
                Logger.LogDebug("Business type changing from '{OldBizType}' to '{NewBizType}' for token ID {Id}", 
                    oldBizType, bizType, id);
                
                var minIdInfo = await MinIdInfoRepository.FindByBizTypeAsync(bizType, cancellationToken);
                if (minIdInfo == null)
                {
                    var errorMessage = $"Cannot update MinId token: Business type '{bizType}' is not registered in the system. Please ensure the business type configuration exists.";
                    Logger.LogWarning("Failed to update MinId token due to missing business type: {BizType}", bizType);
                    throw new AbpException(errorMessage);
                }
            }

            // Check for token uniqueness within the business type context (excluding current token)
            var queryMinIdToken = await MinIdTokenRepository.FindExpectedByTokenAsync(bizType, token, id, cancellationToken);
            if (queryMinIdToken != null)
            {
                var errorMessage = $"Cannot update MinId token: Token '{token}' already exists for business type '{bizType}' in another record (ID: {queryMinIdToken.Id}). Each token must be unique within its business type context.";
                Logger.LogWarning("Failed to update MinId token due to duplicate token: {Token} for business type {BizType} (existing ID: {ExistingId})", 
                    token, bizType, queryMinIdToken.Id);
                throw new AbpException(errorMessage);
            }

            minIdToken.Update(bizType, token, remark);

            var result = await MinIdTokenRepository.UpdateAsync(minIdToken, cancellationToken: cancellationToken);
            Logger.LogInformation("Successfully updated MinId token {Id}: business type changed from '{OldBizType}' to '{NewBizType}', token changed from '{OldToken}' to '{NewToken}', remark: '{Remark}'", 
                id, oldBizType, bizType, oldToken, token, remark);
            
            return result;
        }
    }
}
