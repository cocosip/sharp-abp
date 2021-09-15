using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer.Clients
{
    public class ClientScopeDto : EntityDto
    {
        /// <summary>
        /// ClientId
        /// </summary>
        public Guid ClientId { get; set; }

        /// <summary>
        /// Scope
        /// </summary>
        public string Scope { get; set; }
    }
}
