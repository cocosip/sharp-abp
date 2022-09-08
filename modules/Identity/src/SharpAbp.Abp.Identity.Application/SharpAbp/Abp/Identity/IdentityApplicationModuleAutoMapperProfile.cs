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

            CreateMap<IdentityLockoutSettings, IdentityLockoutSettingsDto>();
            CreateMap<IdentityPasswordSettings, IdentityPasswordSettingsDto>();
            CreateMap<IdentitySignInSettings, IdentitySignInSettingsDto>();
            CreateMap<IdentityUserSettings, IdentityUserSettingsDto>();

            CreateMap<UpdateIdentityLockoutSettingsDto, IdentityLockoutSettings>();
            CreateMap<UpdateIdentityPasswordSettingsDto, IdentityPasswordSettings>();
            CreateMap<UpdateIdentitySignInSettingsDto, IdentitySignInSettings>();
            CreateMap<UpdateIdentityUserSettingsDto, IdentityUserSettings>();
        }
    }
}
