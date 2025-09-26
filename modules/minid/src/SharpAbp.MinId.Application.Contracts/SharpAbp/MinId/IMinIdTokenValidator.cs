﻿﻿using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Defines the contract for validating MinId authentication tokens.
    /// Provides methods to verify the validity of tokens for specific business types.
    /// </summary>
    public interface IMinIdTokenValidator
    {
        /// <summary>
        /// Asynchronously validates whether a token is valid for a given business type.
        /// </summary>
        /// <param name="bizType">The business type identifier associated with the token. Cannot be null, empty, or whitespace.</param>
        /// <param name="token">The authentication token to validate. Cannot be null, empty, or whitespace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>True if the token is valid for the specified business type; otherwise, false.</returns>
        Task<bool> ValidateAsync([NotNull] string bizType, [NotNull] string token, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously validates a token for a given business type and throws an exception if invalid.
        /// </summary>
        /// <param name="bizType">The business type identifier associated with the token. Cannot be null, empty, or whitespace.</param>
        /// <param name="token">The authentication token to validate. Cannot be null, empty, or whitespace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <exception cref="Volo.Abp.AbpException">Thrown when the token is invalid for the specified business type.</exception>
        Task ValidateTokenAsync([NotNull] string bizType, [NotNull] string token, CancellationToken cancellationToken = default);
    }
}