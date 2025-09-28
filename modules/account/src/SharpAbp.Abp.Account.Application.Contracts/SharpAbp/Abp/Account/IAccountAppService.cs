using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Account
{
    /// <summary>
    /// Application service interface for account management operations.
    /// Provides user registration, password reset functionality.
    /// </summary>
    public interface IAccountAppService : IApplicationService
    {
        /// <summary>
        /// Registers a new user account in the system.
        /// </summary>
        /// <param name="input">The registration input containing user details and password.</param>
        /// <returns>The created user information as IdentityUserDto.</returns>
        Task<IdentityUserDto> RegisterAsync(RegisterDto input);

        /// <summary>
        /// Sends a password reset code to the specified email address.
        /// </summary>
        /// <param name="input">The input containing email and optional app information for the reset link.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input);

        /// <summary>
        /// Resets a user's password using the provided reset token.
        /// </summary>
        /// <param name="input">The input containing user ID, reset token, and new password.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ResetPasswordAsync(ResetPasswordDto input);
    }
}
