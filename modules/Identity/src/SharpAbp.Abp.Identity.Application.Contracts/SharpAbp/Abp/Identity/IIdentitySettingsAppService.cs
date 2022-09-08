using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.Identity
{
    public interface IIdentitySettingsAppService : IApplicationService
    {
        Task<IdentitySettingsDto> GetAsync();
        Task UpdateAsync(UpdateIdentitySettingsDto input);
    }
}
