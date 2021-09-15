using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer
{
    public abstract class SecretDto : EntityDto
    {
        public string Type { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public DateTime? Expiration { get; set; }
    }
}
