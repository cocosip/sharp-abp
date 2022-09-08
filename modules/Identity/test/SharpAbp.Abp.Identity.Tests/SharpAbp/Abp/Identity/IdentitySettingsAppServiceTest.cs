using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.Identity
{
    public class IdentitySettingsAppServiceTest : IdentityApplicationTestBase
    {
        private readonly IIdentitySettingsAppService _identitySettingsAppService;
        public IdentitySettingsAppServiceTest()
        {
            _identitySettingsAppService = GetRequiredService<IIdentitySettingsAppService>();
        }

        [Fact]
        public virtual async Task Get_IdentitySettings_TestAsync()
        {
            var settings = await _identitySettingsAppService.GetAsync();

            Assert.True(settings.Lockout.AllowedForNewUsers);
            Assert.Equal(300, settings.Lockout.LockoutDuration);
            Assert.Equal(5, settings.Lockout.MaxFailedAccessAttempts);

            Assert.True(settings.Password.RequireDigit);
            Assert.True(settings.Password.RequireLowercase);
            Assert.True(settings.Password.RequireNonAlphanumeric);
            Assert.True(settings.Password.RequireUppercase);
            Assert.Equal(6, settings.Password.RequiredLength);
            Assert.Equal(1, settings.Password.RequiredUniqueChars);

            Assert.False(settings.SignIn.RequireConfirmedEmail);
            Assert.False(settings.SignIn.RequireConfirmedPhoneNumber);
            Assert.True(settings.SignIn.EnablePhoneNumberConfirmation);

            Assert.True(settings.User.IsUserNameUpdateEnabled);
            Assert.True(settings.User.IsEmailUpdateEnabled);
        }

        [Fact]
        public virtual async Task Update_IdentitySettings_TestAsync()
        {

            var updateSettings = new UpdateIdentitySettingsDto()
            {
                Lockout = new UpdateIdentityLockoutSettingsDto()
                {
                    AllowedForNewUsers = false,
                    LockoutDuration = 600,
                    MaxFailedAccessAttempts = 3
                },
                Password = new UpdateIdentityPasswordSettingsDto()
                {
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireNonAlphanumeric = false,
                    RequireUppercase = false,
                    RequiredLength = 8,
                    RequiredUniqueChars = 2
                },
                SignIn = new UpdateIdentitySignInSettingsDto()
                {
                    RequireConfirmedEmail = true,
                    RequireConfirmedPhoneNumber = true,
                    EnablePhoneNumberConfirmation = false
                },
                User = new UpdateIdentityUserSettingsDto()
                {
                    IsUserNameUpdateEnabled = false,
                    IsEmailUpdateEnabled = false
                }
            };
            await _identitySettingsAppService.UpdateAsync(updateSettings);
            var settings = await _identitySettingsAppService.GetAsync();

            Assert.False(settings.Lockout.AllowedForNewUsers);
            Assert.Equal(600, settings.Lockout.LockoutDuration);
            Assert.Equal(3, settings.Lockout.MaxFailedAccessAttempts);

            Assert.False(settings.Password.RequireDigit);
            Assert.False(settings.Password.RequireLowercase);
            Assert.False(settings.Password.RequireNonAlphanumeric);
            Assert.False(settings.Password.RequireUppercase);
            Assert.Equal(8, settings.Password.RequiredLength);
            Assert.Equal(2, settings.Password.RequiredUniqueChars);

            Assert.True(settings.SignIn.RequireConfirmedEmail);
            Assert.True(settings.SignIn.RequireConfirmedPhoneNumber);
            Assert.False(settings.SignIn.EnablePhoneNumberConfirmation);

            Assert.False(settings.User.IsUserNameUpdateEnabled);
            Assert.False(settings.User.IsEmailUpdateEnabled);
        }
    }
}
