using System.Runtime.Serialization;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Identity;
using Volo.Abp.Mapperly;

namespace SharpAbp.Abp.Identity
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class IdentityClaimTypeToIdentityClaimTypeDtoMapper : MapperBase<IdentityClaimType, IdentityClaimTypeDto>
    {

        [MapperIgnoreTarget(nameof(IdentityClaimTypeDto.ValueTypeAsString))]
        public override partial IdentityClaimTypeDto Map(IdentityClaimType source);

        [MapperIgnoreTarget(nameof(IdentityClaimTypeDto.ValueTypeAsString))]
        public override partial void Map(IdentityClaimType source, IdentityClaimTypeDto destination);

        public override void AfterMap(IdentityClaimType source, IdentityClaimTypeDto destination)
        {
            destination.ValueTypeAsString = source.ValueType.ToString();
        }
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class IdentityUserClaimToIdentityUserClaimDtoMapper : MapperBase<IdentityUserClaim, IdentityUserClaimDto>
    {
        public override partial IdentityUserClaimDto Map(IdentityUserClaim source);
        public override partial void Map(IdentityUserClaim source, IdentityUserClaimDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class IdentityRoleClaimToIdentityRoleClaimDtoMapper : MapperBase<IdentityRoleClaim, IdentityRoleClaimDto>
    {
        public override partial IdentityRoleClaimDto Map(IdentityRoleClaim source);
        public override partial void Map(IdentityRoleClaim source, IdentityRoleClaimDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class IdentitySecurityLogToIdentitySecurityLogDtoMapper : MapperBase<IdentitySecurityLog, IdentitySecurityLogDto>
    {
        public override partial IdentitySecurityLogDto Map(IdentitySecurityLog source);
        public override partial void Map(IdentitySecurityLog source, IdentitySecurityLogDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class OrganizationUnitToOrganizationUnitDtoMapper : MapperBase<OrganizationUnit, OrganizationUnitDto>
    {
        public override partial OrganizationUnitDto Map(OrganizationUnit source);
        public override partial void Map(OrganizationUnit source, OrganizationUnitDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class OrganizationUnitRoleToOrganizationUnitRoleDtoMapper : MapperBase<OrganizationUnitRole, OrganizationUnitRoleDto>
    {
        public override partial OrganizationUnitRoleDto Map(OrganizationUnitRole source);
        public override partial void Map(OrganizationUnitRole source, OrganizationUnitRoleDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class IdentityLockoutSettingsToIdentityLockoutSettingsDtoMapper : MapperBase<IdentityLockoutSettings, IdentityLockoutSettingsDto>
    {
        public override partial IdentityLockoutSettingsDto Map(IdentityLockoutSettings source);
        public override partial void Map(IdentityLockoutSettings source, IdentityLockoutSettingsDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class IdentityPasswordSettingsToIdentityPasswordSettingsDtoMapper : MapperBase<IdentityPasswordSettings, IdentityPasswordSettingsDto>
    {
        public override partial IdentityPasswordSettingsDto Map(IdentityPasswordSettings source);
        public override partial void Map(IdentityPasswordSettings source, IdentityPasswordSettingsDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class IdentitySignInSettingsToIdentitySignInSettingsDtoMapper : MapperBase<IdentitySignInSettings, IdentitySignInSettingsDto>
    {
        public override partial IdentitySignInSettingsDto Map(IdentitySignInSettings source);
        public override partial void Map(IdentitySignInSettings source, IdentitySignInSettingsDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class IdentityUserSettingsToIdentityUserSettingsDtoMapper : MapperBase<IdentityUserSettings, IdentityUserSettingsDto>
    {
        public override partial IdentityUserSettingsDto Map(IdentityUserSettings source);
        public override partial void Map(IdentityUserSettings source, IdentityUserSettingsDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class UpdateIdentityLockoutSettingsDtoToIdentityLockoutSettingsMapper : MapperBase<UpdateIdentityLockoutSettingsDto, IdentityLockoutSettings>
    {
        public override partial IdentityLockoutSettings Map(UpdateIdentityLockoutSettingsDto source);
        public override partial void Map(UpdateIdentityLockoutSettingsDto source, IdentityLockoutSettings destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class UpdateIdentityPasswordSettingsDtoToIdentityPasswordSettingsMapper : MapperBase<UpdateIdentityPasswordSettingsDto, IdentityPasswordSettings>
    {
        public override partial IdentityPasswordSettings Map(UpdateIdentityPasswordSettingsDto source);
        public override partial void Map(UpdateIdentityPasswordSettingsDto source, IdentityPasswordSettings destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class UpdateIdentitySignInSettingsDtoToIdentitySignInSettingsMapper : MapperBase<UpdateIdentitySignInSettingsDto, IdentitySignInSettings>
    {
        public override partial IdentitySignInSettings Map(UpdateIdentitySignInSettingsDto source);
        public override partial void Map(UpdateIdentitySignInSettingsDto source, IdentitySignInSettings destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class UpdateIdentityUserSignInSettingsDtoToIdentitySignInSettingsMapper : MapperBase<UpdateIdentityUserSettingsDto, IdentityUserSettings>
    {
        public override partial IdentityUserSettings Map(UpdateIdentityUserSettingsDto source);
        public override partial void Map(UpdateIdentityUserSettingsDto source, IdentityUserSettings destination);
    }

    // public class IdentityApplicationModuleAutoMapperProfile : Profile
    // {
    //     public IdentityApplicationModuleAutoMapperProfile()
    //     {
    //         CreateMap<IdentityClaimType, IdentityClaimTypeDto>()
    //             .ForMember(t => t.ValueTypeAsString, s => s.MapFrom(x => x.ValueType.ToString()));

    //         CreateMap<IdentityUserClaim, IdentityUserClaimDto>();
    //         CreateMap<IdentityRoleClaim, IdentityRoleClaimDto>();
    //         CreateMap<IdentitySecurityLog, IdentitySecurityLogDto>();

    //         CreateMap<OrganizationUnit, OrganizationUnitDto>();
    //         CreateMap<OrganizationUnitRole, OrganizationUnitRoleDto>();
    //         CreateMap<IdentityLockoutSettings, IdentityLockoutSettingsDto>();
    //         CreateMap<IdentityPasswordSettings, IdentityPasswordSettingsDto>();
    //         CreateMap<IdentitySignInSettings, IdentitySignInSettingsDto>();
    //         CreateMap<IdentityUserSettings, IdentityUserSettingsDto>();

    //         CreateMap<UpdateIdentityLockoutSettingsDto, IdentityLockoutSettings>();
    //         CreateMap<UpdateIdentityPasswordSettingsDto, IdentityPasswordSettings>();
    //         CreateMap<UpdateIdentitySignInSettingsDto, IdentitySignInSettings>();
    //         CreateMap<UpdateIdentityUserSettingsDto, IdentityUserSettings>();
    //     }
    // }
}
