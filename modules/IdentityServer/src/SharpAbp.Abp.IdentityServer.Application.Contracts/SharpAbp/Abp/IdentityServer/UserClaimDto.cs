using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.IdentityServer
{
    public abstract class UserClaimDto : EntityDto
    {
        public string Type { get; set; }
    }
}
