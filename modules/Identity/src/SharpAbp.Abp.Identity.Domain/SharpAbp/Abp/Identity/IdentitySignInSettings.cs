namespace SharpAbp.Abp.Identity
{
    public class IdentitySignInSettings
    {
        public bool EnablePhoneNumberConfirmation { get; set; }
        public bool RequireConfirmedEmail { get; set; }
        public bool RequireConfirmedPhoneNumber { get; set; }
    }
}
