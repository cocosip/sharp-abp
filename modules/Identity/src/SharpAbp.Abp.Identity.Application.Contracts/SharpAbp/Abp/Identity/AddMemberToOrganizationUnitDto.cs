using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.Identity
{
    public class AddMemberToOrganizationUnitDto
    {
        public List<Guid> UserIds { get; set; }
        public AddMemberToOrganizationUnitDto()
        {
            UserIds = new List<Guid>();
        }
    }
}
