using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Defines the contract for MinId information management operations.
    /// This interface provides domain-level business logic for creating and updating
    /// MinId configurations with proper validation and business rule enforcement.
    /// </summary>
    public interface IMinIdInfoManager
    {
        /// <summary>
        /// Creates a new MinId information record with validation to ensure business type uniqueness.
        /// This method enforces business rules to prevent duplicate business types within the system.
        /// </summary>
        /// <param name="minIdInfo">The MinId information entity to create. Cannot be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task containing the created MinId information entity.</returns>
        /// <exception cref="Volo.Abp.AbpException">Thrown when a MinId configuration with the same business type already exists.</exception>
        Task<MinIdInfo> CreateAsync(MinIdInfo minIdInfo, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing MinId information record with new configuration values.
        /// This method validates that the updated business type doesn't conflict with existing records
        /// and applies the changes to the specified MinId configuration.
        /// </summary>
        /// <param name="id">The unique identifier of the MinId configuration to update.</param>
        /// <param name="bizType">The business type identifier for the configuration.</param>
        /// <param name="maxId">The maximum ID value for the configuration.</param>
        /// <param name="step">The step size for ID allocation.</param>
        /// <param name="delta">The delta value for modulo operations in distributed scenarios.</param>
        /// <param name="remainder">The remainder value for modulo operations.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task containing the updated MinId information entity.</returns>
        /// <exception cref="Volo.Abp.AbpException">Thrown when the specified ID is not found or when the business type conflicts with existing records.</exception>
        Task<MinIdInfo> UpdateAsync(Guid id, string bizType, long maxId, int step, int delta, int remainder, CancellationToken cancellationToken = default);
    }
}