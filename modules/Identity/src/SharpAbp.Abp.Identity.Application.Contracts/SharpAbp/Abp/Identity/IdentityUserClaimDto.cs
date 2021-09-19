using System;

namespace SharpAbp.Abp.Identity
{
    public class IdentityUserClaimDto : IdentityClaimDto
    {
        public Guid UserId { get; set; }
  
    }
}
