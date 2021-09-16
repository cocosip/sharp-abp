using System;

namespace SharpAbp.Abp.IdentityServer.IdentityResources
{
    public class IdentityResourceClaimDto : UserClaimDto
    {
        public Guid IdentityResourceId { get; set; }
    }
}
