using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Defines the contract for MinId token management operations.
    /// This interface provides domain-level business logic for creating and updating
    /// MinId authentication tokens with proper validation and business rule enforcement.
    /// </summary>
    public interface IMinIdTokenManager
    {
        /// <summary>
        /// Creates a new MinId authentication token with validation to ensure token uniqueness
        /// within the specified business type context. This method also validates that the
        /// associated business type exists in the system before creating the token.
        /// </summary>
        /// <param name="minIdToken">The MinId token entity to create. Cannot be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task containing the created MinId token entity.</returns>
        /// <exception cref="Volo.Abp.AbpException">
        /// Thrown when:
        /// - The associated business type is not found in the system
        /// - A token with the same value already exists for the specified business type
        /// </exception>
        Task<MinIdToken> CreateAsync(MinIdToken minIdToken, CancellationToken cancellationToken = default);

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
        /// <exception cref="Volo.Abp.AbpException">
        /// Thrown when:
        /// - The specified token ID is not found
        /// - The business type is changed but the new business type is not found in the system
        /// - A token with the same value already exists for the specified business type
        /// </exception>
        Task<MinIdToken> UpdateAsync(Guid id, string bizType, string token, string remark, CancellationToken cancellationToken = default);
    }
}