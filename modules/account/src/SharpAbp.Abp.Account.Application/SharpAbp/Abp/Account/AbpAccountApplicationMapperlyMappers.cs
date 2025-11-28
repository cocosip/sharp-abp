using AutoMapper;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Mapperly;

namespace SharpAbp.Abp.Account
{

    // [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    // [MapExtraProperties]
    // public partial class IdentityUserToProfileDtoMapper : MapperBase<IdentityUser, ProfileDto>
    // {
    //     [MapPropertyFromSource(nameof(ProfileDto.HasPassword), Use = "CustomHasPassword")]
    //     public override partial ProfileDto Map(IdentityUser source);

    //     [MapPropertyFromSource(nameof(ProfileDto.HasPassword), Use = "CustomHasPassword")]
    //     public override partial void Map(IdentityUser source, ProfileDto destination);


    //     [NamedMapping("CustomHasPassword")]
    //     private bool HasPassword(IdentityUser user) => user.PasswordHash != null;
    // }


    // [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    // public partial class IdentityUserToIdentityUserDtoMapper : MapperBase<Volo.Abp.Identity.IdentityUser, Volo.Abp.Identity.IdentityUserDto>
    // {
    //     public override partial IdentityUserDto Map(IdentityUser source);

    //     public override partial void Map(IdentityUser source, IdentityUserDto destination);
    // }


    public class AbpAccountApplicationModuleAutoMapperProfile : Profile
    {
        public AbpAccountApplicationModuleAutoMapperProfile()
        {
            CreateMap<IdentityUser, ProfileDto>()
                .ForMember(dest => dest.HasPassword,
                    op => op.MapFrom(src => src.PasswordHash != null))
                .MapExtraProperties();

            CreateMap<IdentityUser, IdentityUserDto>()
                .MapExtraProperties();

        }
    }
}