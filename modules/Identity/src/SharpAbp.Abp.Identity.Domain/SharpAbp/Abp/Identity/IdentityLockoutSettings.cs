namespace SharpAbp.Abp.Identity
{
    public class IdentityLockoutSettings
    {
        public bool AllowedForNewUsers { get; set; }
        public int LockoutDuration { get; set; }
        public int MaxFailedAccessAttempts { get; set; }
    }
}
