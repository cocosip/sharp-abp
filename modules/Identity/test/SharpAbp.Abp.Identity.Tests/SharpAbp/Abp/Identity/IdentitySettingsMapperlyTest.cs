using Xunit;

namespace SharpAbp.Abp.Identity;

public class IdentitySettingsMapperlyTest
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Lockout_Settings_Map_Both_Overloads(bool allowed)
    {
        var update = new UpdateIdentityLockoutSettingsDto { AllowedForNewUsers = allowed, LockoutDuration = 713, MaxFailedAccessAttempts = 9 };
        var fromDto = new UpdateIdentityLockoutSettingsDtoToIdentityLockoutSettingsMapper();
        var settings = fromDto.Map(update);
        var existing = new IdentityLockoutSettings();
        fromDto.Map(update, existing);
        var toDto = new IdentityLockoutSettingsToIdentityLockoutSettingsDtoMapper();
        var dto = toDto.Map(settings);
        var destination = new IdentityLockoutSettingsDto();
        toDto.Map(existing, destination);

        foreach (var result in new[] { dto, destination })
        {
            Assert.Equal(allowed, result.AllowedForNewUsers);
            Assert.Equal(713, result.LockoutDuration);
            Assert.Equal(9, result.MaxFailedAccessAttempts);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Password_Settings_Map_Both_Overloads(bool required)
    {
        var update = new UpdateIdentityPasswordSettingsDto
        {
            RequireDigit = required, RequireLowercase = !required, RequireNonAlphanumeric = required,
            RequireUppercase = !required, RequiredLength = 13, RequiredUniqueChars = 5
        };
        var fromDto = new UpdateIdentityPasswordSettingsDtoToIdentityPasswordSettingsMapper();
        var settings = fromDto.Map(update);
        var existing = new IdentityPasswordSettings();
        fromDto.Map(update, existing);
        var toDto = new IdentityPasswordSettingsToIdentityPasswordSettingsDtoMapper();
        var dto = toDto.Map(settings);
        var destination = new IdentityPasswordSettingsDto();
        toDto.Map(existing, destination);

        foreach (var result in new[] { dto, destination })
        {
            Assert.Equal(required, result.RequireDigit);
            Assert.Equal(!required, result.RequireLowercase);
            Assert.Equal(required, result.RequireNonAlphanumeric);
            Assert.Equal(!required, result.RequireUppercase);
            Assert.Equal(13, result.RequiredLength);
            Assert.Equal(5, result.RequiredUniqueChars);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SignIn_Settings_Map_Both_Overloads(bool required)
    {
        var update = new UpdateIdentitySignInSettingsDto
        {
            EnablePhoneNumberConfirmation = required, RequireConfirmedEmail = !required, RequireConfirmedPhoneNumber = required
        };
        var fromDto = new UpdateIdentitySignInSettingsDtoToIdentitySignInSettingsMapper();
        var settings = fromDto.Map(update);
        var existing = new IdentitySignInSettings();
        fromDto.Map(update, existing);
        var toDto = new IdentitySignInSettingsToIdentitySignInSettingsDtoMapper();
        var dto = toDto.Map(settings);
        var destination = new IdentitySignInSettingsDto();
        toDto.Map(existing, destination);

        foreach (var result in new[] { dto, destination })
        {
            Assert.Equal(required, result.EnablePhoneNumberConfirmation);
            Assert.Equal(!required, result.RequireConfirmedEmail);
            Assert.Equal(required, result.RequireConfirmedPhoneNumber);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void User_Settings_Map_Both_Overloads(bool enabled)
    {
        var update = new UpdateIdentityUserSettingsDto { IsEmailUpdateEnabled = enabled, IsUserNameUpdateEnabled = !enabled };
        var fromDto = new UpdateIdentityUserSettingsDtoToIdentityUserSettingsMapper();
        var settings = fromDto.Map(update);
        var existing = new IdentityUserSettings();
        fromDto.Map(update, existing);
        var toDto = new IdentityUserSettingsToIdentityUserSettingsDtoMapper();
        var dto = toDto.Map(settings);
        var destination = new IdentityUserSettingsDto();
        toDto.Map(existing, destination);

        foreach (var result in new[] { dto, destination })
        {
            Assert.Equal(enabled, result.IsEmailUpdateEnabled);
            Assert.Equal(!enabled, result.IsUserNameUpdateEnabled);
        }
    }
}
