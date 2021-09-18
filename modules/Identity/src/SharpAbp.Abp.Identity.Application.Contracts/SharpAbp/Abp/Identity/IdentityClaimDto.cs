using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.Identity
{
    public abstract class IdentityClaimDto : EntityDto<Guid>
    {
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        public string ClaimValue { get; set; }
    }
}
