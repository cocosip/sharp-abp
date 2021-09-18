using System;

namespace SharpAbp.Abp.Identity
{
    public class IdentityRoleClaimDto : IdentityClaimDto
    {
        /// <summary>
        /// Gets or sets the of the primary key of the role associated with this claim.
        /// </summary>
        public Guid RoleId { get; set; }
    }
}
