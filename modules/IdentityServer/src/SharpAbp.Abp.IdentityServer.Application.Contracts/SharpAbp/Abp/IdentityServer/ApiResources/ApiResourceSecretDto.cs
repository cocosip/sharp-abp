using System;

namespace SharpAbp.Abp.IdentityServer.ApiResources
{
    public class ApiResourceSecretDto : SecretDto
    {
        public Guid ApiResourceId { get; set; }
    }
}
