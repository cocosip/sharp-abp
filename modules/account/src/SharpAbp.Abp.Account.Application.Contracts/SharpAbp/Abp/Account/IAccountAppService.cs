using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Account
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IdentityUserDto> RegisterAsync(RegisterDto input);

        Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input);

        Task ResetPasswordAsync(ResetPasswordDto input);
    }
}
