using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharpAbp.Abp.Account.Web;
using SharpAbp.Abp.Account.Web.Modules.Account.Components.Toolbar.UserLoginLink;
using SharpAbp.Abp.Account.Web.Pages.Account;
using SharpAbp.Abp.Account.Web.Pages.Account.Components.ProfileManagementGroup.Password;
using SharpAbp.Abp.Account.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using SharpAbp.Abp.Account.Web.ProfileManagement;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.Abp.Account;

public class AccountWebProfileTest : AccountWebTest
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Profile_Only_Offers_Password_Changes_For_Local_Accounts(bool external)
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var user = await CreateUserAsync(external: external);
        CurrentUser.Id.Returns(user.Id);
        var context = new ProfileManagementPageCreationContext(PageServices);

        await new AccountProfileManagementPageContributor().ConfigureAsync(context);

        Assert.Equal(external ? 1 : 2, context.Groups.Count);
        Assert.Equal(typeof(AccountProfilePersonalInfoManagementGroupViewComponent), context.Groups.Last().ComponentType);
        Assert.Equal(!external, context.Groups.Any(group => group.Id == "Volo.Abp.Account.Password"));
        Assert.All(context.Groups, group => Assert.False(string.IsNullOrWhiteSpace(group.DisplayName)));
    }

    [Fact]
    public void Profile_Groups_Validate_Required_Values_And_Preserve_Parameters()
    {
        var parameter = new object();
        var group = new ProfileManagementPageGroup("personal", "Personal", typeof(AccountProfilePersonalInfoManagementGroupViewComponent), parameter);
        Assert.Equal("personal", group.Id);
        Assert.Equal("Personal", group.DisplayName);
        Assert.Same(parameter, group.Parameter);
        Assert.Throws<ArgumentException>(() => group.Id = " ");
        Assert.Throws<ArgumentException>(() => group.DisplayName = "");
        Assert.Throws<ArgumentNullException>(() => group.ComponentType = null);
    }

    [Fact]
    public async Task Manage_Page_Runs_Contributors_In_Registration_Order()
    {
        var options = new ProfileManagementPageOptions();
        var first = Substitute.For<IProfileManagementPageContributor>();
        var second = Substitute.For<IProfileManagementPageContributor>();
        first.ConfigureAsync(Arg.Do<ProfileManagementPageCreationContext>(context =>
            context.Groups.Add(new ProfileManagementPageGroup("first", "First", typeof(UserLoginLinkViewComponent))))).Returns(Task.CompletedTask);
        second.ConfigureAsync(Arg.Do<ProfileManagementPageCreationContext>(context =>
            context.Groups.Add(new ProfileManagementPageGroup("second", "Second", typeof(UserLoginLinkViewComponent))))).Returns(Task.CompletedTask);
        options.Contributors.Add(first);
        options.Contributors.Add(second);
        var page = Page(new ManageModel(Options.Create(options)) { ReturnUrl = "/return" });

        Assert.IsType<PageResult>(await page.OnGetAsync());
        Assert.Equal(new[] { "first", "second" }, page.ProfileManagementPageCreationContext.Groups.Select(group => group.Id));
        Assert.Same(PageServices, page.ProfileManagementPageCreationContext.ServiceProvider);
        Assert.Equal("/return", page.ReturnUrl);
        Assert.IsType<PageResult>(await page.OnPostAsync());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Password_Component_Only_Requests_An_Old_Password_When_One_Exists(bool hasPassword)
    {
        var profile = Substitute.For<IProfileAppService>();
        profile.GetAsync().Returns(new ProfileDto { HasPassword = hasPassword });
        var component = new AccountProfilePasswordManagementGroupViewComponent(profile);

        var result = Assert.IsType<ViewViewComponentResult>(await component.InvokeAsync());
        var model = Assert.IsType<AccountProfilePasswordManagementGroupViewComponent.ChangePasswordInfoModel>(result.ViewData.Model);
        Assert.Equal(!hasPassword, model.HideOldPasswordInput);
        Assert.EndsWith("/Password/Default.cshtml", result.ViewName);
        model.CurrentPassword = "CurrentP@ssword";
        model.NewPassword = "NewP@ssword";
        model.NewPasswordConfirm = "NewP@ssword";
        var errors = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), errors, true));
        Assert.Equal(model.NewPassword, model.NewPasswordConfirm);
    }

    [Fact]
    public async Task Personal_Info_Component_Uses_The_Web_Object_Mapper()
    {
        var profile = Substitute.For<IProfileAppService>();
        var dto = new ProfileDto { UserName = "web-user", Email = "web@example.com" };
        profile.GetAsync().Returns(dto);
        var mapper = Substitute.For<IObjectMapper<AccountWebModule>>();
        var mapped = new AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel { UserName = dto.UserName, Email = dto.Email };
        mapper.Map<ProfileDto, AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel>(dto).Returns(mapped);
        ServiceOverrides[typeof(IObjectMapper<AccountWebModule>)] = mapper;
        var component = new AccountProfilePersonalInfoManagementGroupViewComponent(profile)
        {
            LazyServiceProvider = new AbpLazyServiceProvider(PageServices)
        };

        var result = Assert.IsType<ViewViewComponentResult>(await component.InvokeAsync());
        Assert.Same(mapped, result.ViewData.Model);
        Assert.EndsWith("/PersonalInfo/Default.cshtml", result.ViewName);
    }

    [Theory]
    [InlineData(StandardToolbars.Main, false, true)]
    [InlineData(StandardToolbars.Main, true, false)]
    [InlineData("other", false, false)]
    public async Task Toolbar_Only_Shows_The_Login_Link_To_Anonymous_Main_Toolbar_Users(string name, bool authenticated, bool expected)
    {
        CurrentUser.IsAuthenticated.Returns(authenticated);
        var toolbar = new Toolbar(name);
        await new AccountModuleToolbarContributor().ConfigureToolbarAsync(new ToolbarConfigurationContext(null, toolbar, PageServices));
        Assert.Equal(expected ? 1 : 0, toolbar.Items.Count);
        if (expected)
        {
            Assert.Equal(typeof(UserLoginLinkViewComponent), toolbar.Items[0].ComponentType);
        }
        Assert.EndsWith("/UserLoginLink/Default.cshtml", Assert.IsType<ViewViewComponentResult>(new UserLoginLinkViewComponent().Invoke()).ViewName);
    }

    [Theory]
    [InlineData(StandardMenus.User, 2)]
    [InlineData("other", 0)]
    public async Task User_Menu_Adds_Profile_And_Logout_Links_Only_To_The_User_Menu(string name, int count)
    {
        var menu = new ApplicationMenu(name);
        await new AbpAccountUserMenuContributor().ConfigureMenuAsync(new MenuConfigurationContext(menu, PageServices));
        Assert.Equal(count, menu.Items.Count);
        if (count > 0)
        {
            Assert.Equal("~/Account/Manage", menu.Items[0].Url);
            Assert.Equal("~/Account/Logout", menu.Items[1].Url);
        }
    }
}
