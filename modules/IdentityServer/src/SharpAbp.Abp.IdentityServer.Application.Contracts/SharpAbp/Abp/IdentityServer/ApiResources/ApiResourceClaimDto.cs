using System;

namespace SharpAbp.Abp.IdentityServer.ApiResources
{
    public class ApiResourceClaimDto : UserClaimDto
    {
        public Guid ApiResourceId { get; set; }
    }
}
