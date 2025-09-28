using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.Account
{
    /// <summary>
    /// Application service interface for managing user profile operations.
    /// Provides functionality for retrieving, updating user profiles and changing passwords.
    /// </summary>
    public interface IProfileAppService : IApplicationService
    {
        /// <summary>
        /// Gets the current user's profile information.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user's profile data.</returns>
        Task<ProfileDto> GetAsync();

        /// <summary>
        /// Updates the current user's profile information.
        /// </summary>
        /// <param name="input">The profile update data containing new user information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated profile data.</returns>
        Task<ProfileDto> UpdateAsync(UpdateProfileDto input);

        /// <summary>
        /// Changes the current user's password.
        /// </summary>
        /// <param name="input">The password change data containing current and new passwords.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ChangePasswordAsync(ChangePasswordInput input);
    }
}