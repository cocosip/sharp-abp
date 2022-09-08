using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Identity
{
    [Authorize(IdentityPermissions.Settings.Default)]
    public class IdentitySettingsAppService : IdentityAppServiceBase, IIdentitySettingsAppService
    {
        protected IdentitySettingsManager SettingsManager { get; set; }
        public IdentitySettingsAppService(IdentitySettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        [Authorize(IdentityPermissions.Settings.Default)]
        public virtual async Task<IdentitySettingsDto> GetAsync()
        {
            var identitySettingsDto = new IdentitySettingsDto();
            var lockout = await SettingsManager.GetLockoutSettingsAsync();
            var password = await SettingsManager.GetPasswordSettingsAsync();
            var signIn = await SettingsManager.GetSignInSettingsAsync();
            var user = await SettingsManager.GetUserSettingsAsync();

            identitySettingsDto.Lockout = ObjectMapper.Map<IdentityLockoutSettings, IdentityLockoutSettingsDto>(lockout);
            identitySettingsDto.Password = ObjectMapper.Map<IdentityPasswordSettings, IdentityPasswordSettingsDto>(password);
            identitySettingsDto.SignIn = ObjectMapper.Map<IdentitySignInSettings, IdentitySignInSettingsDto>(signIn);
            identitySettingsDto.User = ObjectMapper.Map<IdentityUserSettings, IdentityUserSettingsDto>(user);
            return identitySettingsDto;
        }

        [Authorize(IdentityPermissions.Settings.Update)]
        public virtual async Task UpdateAsync(UpdateIdentitySettingsDto input)
        {
            var lockout = ObjectMapper.Map<UpdateIdentityLockoutSettingsDto, IdentityLockoutSettings>(input.Lockout);
            var password = ObjectMapper.Map<UpdateIdentityPasswordSettingsDto, IdentityPasswordSettings>(input.Password);
            var signIn = ObjectMapper.Map<UpdateIdentitySignInSettingsDto, IdentitySignInSettings>(input.SignIn);
            var user = ObjectMapper.Map<UpdateIdentityUserSettingsDto, IdentityUserSettings>(input.User);

            await SettingsManager.SetLockoutSettingsAsync(lockout);
            await SettingsManager.SetPasswordSettingsAsync(password);
            await SettingsManager.SetSignInSettingsAsync(signIn);
            await SettingsManager.SetIdentityUserSettingsAsync(user);
        }

    }
}
