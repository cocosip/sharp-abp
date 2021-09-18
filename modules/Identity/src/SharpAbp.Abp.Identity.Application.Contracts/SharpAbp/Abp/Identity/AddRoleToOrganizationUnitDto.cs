using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.Identity
{
    public class AddRoleToOrganizationUnitDto
    {
        public List<Guid> RoleIds { get; set; }

        public AddRoleToOrganizationUnitDto()
        {
            RoleIds = new List<Guid>();
        }
    }
}
