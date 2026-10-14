using Riok.Mapperly.Abstractions;
using SharpAbp.Abp.Account.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using Volo.Abp.Account;
using Volo.Abp.Mapperly;

namespace SharpAbp.Abp.Account.Web
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ProfileDtoToPersonalInfoModelMapper : MapperBase<ProfileDto, AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel>
    {
        public override partial AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel Map(ProfileDto source);
        public override partial void Map(ProfileDto source, AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel destination);
    }
}
