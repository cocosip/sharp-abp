using Riok.Mapperly.Abstractions;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Mapperly;

namespace SharpAbp.Abp.Account
{

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    [MapExtraProperties]
    public partial class IdentityUserToProfileDtoMapper : MapperBase<IdentityUser, ProfileDto>
    {
        [MapperIgnoreTarget(nameof(ProfileDto.HasPassword))]
        public override partial ProfileDto Map(IdentityUser source);

        [MapperIgnoreTarget(nameof(ProfileDto.HasPassword))]
        public override partial void Map(IdentityUser source, ProfileDto destination);

        public override void AfterMap(IdentityUser source, ProfileDto destination)
        {
            destination.HasPassword = source.PasswordHash != null;
        }
    }

    // public class AbpAccountApplicationModuleAutoMapperProfile : Profile
    // {
    //     public AbpAccountApplicationModuleAutoMapperProfile()
    //     {
    //         CreateMap<IdentityUser, ProfileDto>()
    //             .ForMember(dest => dest.HasPassword,
    //                 op => op.MapFrom(src => src.PasswordHash != null))
    //             .MapExtraProperties();

    //         CreateMap<IdentityUser, IdentityUserDto>()
    //             .MapExtraProperties();

    //     }
    // }
}