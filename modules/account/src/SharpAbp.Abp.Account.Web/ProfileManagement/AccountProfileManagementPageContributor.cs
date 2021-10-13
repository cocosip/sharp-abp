using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SharpAbp.Abp.Account.Localization;
using SharpAbp.Abp.Account.Web.Pages.Account.Components.ProfileManagementGroup.Password;
using SharpAbp.Abp.Account.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace SharpAbp.Abp.Account.Web.ProfileManagement
{
    public class AccountProfileManagementPageContributor : IProfileManagementPageContributor
    {
        public async Task ConfigureAsync(ProfileManagementPageCreationContext context)
        {
            var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<AccountResource>>();

            if (await IsPasswordChangeEnabled(context))
            {
                context.Groups.Add(
                    new ProfileManagementPageGroup(
                        "Volo.Abp.Account.Password",
                        l["ProfileTab:Password"],
                        typeof(AccountProfilePasswordManagementGroupViewComponent)
                    )
                );
            }

            context.Groups.Add(
                new ProfileManagementPageGroup(
                    "Volo.Abp.Account.PersonalInfo",
                    l["ProfileTab:PersonalInfo"],
                    typeof(AccountProfilePersonalInfoManagementGroupViewComponent)
                )
            );
        }

        protected virtual async Task<bool> IsPasswordChangeEnabled(ProfileManagementPageCreationContext context)
        {
            var userManager = context.ServiceProvider.GetRequiredService<IdentityUserManager>();
            var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

            var user = await userManager.GetByIdAsync(currentUser.GetId());

            return !user.IsExternal;
        }
    }
}
