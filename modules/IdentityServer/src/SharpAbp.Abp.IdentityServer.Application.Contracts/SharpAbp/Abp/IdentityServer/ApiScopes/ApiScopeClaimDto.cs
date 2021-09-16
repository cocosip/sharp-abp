using System;

namespace SharpAbp.Abp.IdentityServer.ApiScopes
{
    public class ApiScopeClaimDto : UserClaimDto
    {
        public Guid ApiScopeId { get; set; }
    }
}
