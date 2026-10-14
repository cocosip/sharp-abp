using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Account.Web;
using SharpAbp.Abp.Account.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace SharpAbp.Abp.Account
{
    public class AccountWebMapperlyTest
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Map_Profile_To_PersonalInfo_Uses_Mapperly(bool existingDestination)
        {
            var services = new ServiceCollection();
            using var application = services.AddApplication<AccountWebMappingTestModule>();
            using var provider = services.BuildServiceProvider();
            Assert.IsType<MapperlyAutoObjectMappingProvider>(provider.GetRequiredService<IObjectMapper>().AutoObjectMappingProvider);
            var mapper = provider.GetRequiredService<IObjectMapper<AccountWebModule>>();
            var source = new ProfileDto
            {
                UserName = "review-user",
                Email = "review@example.invalid",
                Name = "Review",
                Surname = "User",
                PhoneNumber = "12345",
                ConcurrencyStamp = "review-stamp"
            };
            var result = existingDestination
                ? mapper.Map(source, new AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel())
                : mapper.Map<ProfileDto, AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel>(source);

            Assert.Equal(source.UserName, result.UserName);
            Assert.Equal(source.Email, result.Email);
            Assert.Equal(source.Name, result.Name);
            Assert.Equal(source.Surname, result.Surname);
            Assert.Equal(source.PhoneNumber, result.PhoneNumber);
            Assert.Equal(source.ConcurrencyStamp, result.ConcurrencyStamp);
        }
    }

    [DependsOn(typeof(AccountApplicationModule), typeof(AccountWebModule))]
    public class AccountWebMappingTestModule : AbpModule
    {
    }
}
