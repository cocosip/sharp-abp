using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Account.Web;
using SharpAbp.Abp.Account.Web.Pages.Account;
using SharpAbp.Abp.Account.Web.ProfileManagement;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Xunit;

namespace SharpAbp.Abp.Account;

public class AccountWebModuleTest
{
    [Fact]
    public void Module_Registers_Profile_Page_Authorization_Assets_And_Navigation()
    {
        var services = new ServiceCollection();
        using var application = services.AddApplication<AccountWebMappingTestModule>();
        using var provider = services.BuildServiceProvider();

        Assert.Contains(provider.GetRequiredService<IOptions<AbpNavigationOptions>>().Value.MenuContributors,
            contributor => contributor is AbpAccountUserMenuContributor);
        Assert.Contains(provider.GetRequiredService<IOptions<AbpToolbarOptions>>().Value.Contributors,
            contributor => contributor is AccountModuleToolbarContributor);
        Assert.Contains(provider.GetRequiredService<IOptions<ProfileManagementPageOptions>>().Value.Contributors,
            contributor => contributor is AccountProfileManagementPageContributor);
        Assert.NotEmpty(provider.GetRequiredService<IOptions<RazorPagesOptions>>().Value.Conventions);
        Assert.NotEmpty(provider.GetRequiredService<IOptions<AbpVirtualFileSystemOptions>>().Value.FileSets);
        var bundles = provider.GetRequiredService<IOptions<AbpBundlingOptions>>().Value.ScriptBundles;
        bundles.Add(typeof(ManageModel).FullName);
        var files = bundles.Get(typeof(ManageModel).FullName).Contributors.GetAll()
            .OfType<BundleFileContributor>().SelectMany(contributor => contributor.Files).Select(file => file.FileName);
        Assert.Contains("/Pages/Account/Components/ProfileManagementGroup/Password/Default.js", files);
        Assert.Contains("/Pages/Account/Components/ProfileManagementGroup/PersonalInfo/Default.js", files);
    }
}
