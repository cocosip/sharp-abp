using System;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    public class NewIdentityUserUpdateDto : IdentityUserUpdateDto
    {
        public Guid[] OrganizationUnitIds { get; set; }
    }
}
