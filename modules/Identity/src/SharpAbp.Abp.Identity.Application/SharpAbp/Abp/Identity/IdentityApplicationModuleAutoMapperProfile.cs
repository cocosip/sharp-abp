using AutoMapper;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    public class IdentityApplicationModuleAutoMapperProfile : Profile
    {
        public IdentityApplicationModuleAutoMapperProfile()
        {
            CreateMap<IdentityClaimType, IdentityClaimTypeDto>();
            CreateMap<IdentityRoleClaim, IdentityRoleClaimDto>();
            CreateMap<IdentitySecurityLog, IdentitySecurityLogDto>();
            CreateMap<OrganizationUnit, OrganizationUnitDto>();
        }
    }
}
