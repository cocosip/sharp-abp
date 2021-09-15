using System;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class ClientSecretDto : SecretDto
    {
        /// <summary>
        /// ClientId
        /// </summary>
        public Guid ClientId { get; set; }
    }
}
