namespace SharpAbp.Abp.Identity
{
    public class IdentityRoleAppServiceTest : IdentityApplicationTestBase
    {
        private readonly IIdentityClaimTypeAppService _identityClaimTypeAppService;
        private readonly IIdentityRoleAppService _identityRoleAppService;
        public IdentityRoleAppServiceTest()
        {
            _identityClaimTypeAppService = GetRequiredService<IIdentityClaimTypeAppService>();
            _identityRoleAppService = GetRequiredService<IIdentityRoleAppService>();
        }


    }
}
