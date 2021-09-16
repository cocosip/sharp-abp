using System;

namespace SharpAbp.Abp.IdentityServer.IdentityResources
{
    public class CreateOrUpdateIdentityResourcePropertyDto
    {
        public Guid? IdentityResourceId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
