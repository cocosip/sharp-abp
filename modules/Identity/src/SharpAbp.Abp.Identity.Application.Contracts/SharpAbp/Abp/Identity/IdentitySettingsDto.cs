namespace SharpAbp.Abp.Identity
{
    public class IdentitySettingsDto
    {
        public IdentityLockoutSettingsDto Lockout { get; set; }
        public IdentityPasswordSettingsDto Password { get; set; }
        public IdentitySignInSettingsDto SignIn { get; set; }
        public IdentityUserSettingsDto User { get; set; }
    }

    public class IdentityLockoutSettingsDto
    {
        public bool AllowedForNewUsers { get; set; }
        public int LockoutDuration { get; set; }
        public int MaxFailedAccessAttempts { get; set; }
    }

    public class IdentityPasswordSettingsDto
    {
        public bool RequireDigit { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public bool RequireUppercase { get; set; }
        public int RequiredLength { get; set; }
        public int RequiredUniqueChars { get; set; }
    }

    public class IdentitySignInSettingsDto
    {
        public bool EnablePhoneNumberConfirmation { get; set; }
        public bool RequireConfirmedEmail { get; set; }
        public bool RequireConfirmedPhoneNumber { get; set; }
    }

    public class IdentityUserSettingsDto
    {
        public bool IsEmailUpdateEnabled { get; set; }
        public bool IsUserNameUpdateEnabled { get; set; }
    }
}
