using AutoMapper;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    public class IdentityApplicationModuleAutoMapperProfile : Profile
    {
        public IdentityApplicationModuleAutoMapperProfile()
        {
            CreateMap<IdentityClaimType, IdentityClaimTypeDto>()
                .ForMember(t => t.ValueTypeAsString, s => s.MapFrom(x => x.ValueType.ToString()));

            CreateMap<IdentityUserClaim, IdentityUserClaimDto>();
            CreateMap<IdentityRoleClaim, IdentityRoleClaimDto>();
            CreateMap<IdentitySecurityLog, IdentitySecurityLogDto>();
            CreateMap<OrganizationUnit, OrganizationUnitDto>();
            CreateMap<OrganizationUnitRole, OrganizationUnitRoleDto>();
        }
    }
}
