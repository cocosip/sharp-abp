using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharpAbp.Abp.Account.Web.Pages.Account;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc.UI.Alerts;
using Volo.Abp.AspNetCore.Mvc.Validation;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Http;
using Volo.Abp.Security.Claims;
using Volo.Abp.SecurityLog;
using Volo.Abp.Settings;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace SharpAbp.Abp.Account;

public abstract class AccountWebTest : AccountApplicationTestBase
{
    protected IdentityUserManager Users { get; }
    protected SignInManager<IdentityUser> SignIn { get; }
    protected IAuthenticationSchemeProvider Schemes { get; } = Substitute.For<IAuthenticationSchemeProvider>();
    protected ISettingProvider Settings { get; } = Substitute.For<ISettingProvider>();
    protected IAccountAppService Account { get; } = Substitute.For<IAccountAppService>();
    protected ICurrentUser CurrentUser { get; } = Substitute.For<ICurrentUser>();
    protected ISecurityLogManager SecurityLogs { get; } = Substitute.For<ISecurityLogManager>();
    protected IdentitySecurityLogManager IdentityLogs { get; }
    protected AlertList Alerts { get; } = new();
    protected IExceptionToErrorInfoConverter Errors { get; } = Substitute.For<IExceptionToErrorInfoConverter>();
    protected IServiceProvider PageServices { get; }
    protected Dictionary<Type, object> ServiceOverrides { get; } = new();

    protected AccountWebTest()
    {
        Users = GetRequiredService<IdentityUserManager>();
        var http = new DefaultHttpContext { RequestServices = ServiceProvider };
        var accessor = new HttpContextAccessor { HttpContext = http };
        SignIn = Substitute.For<SignInManager<IdentityUser>>(Users, accessor,
            GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>(), GetRequiredService<IOptions<IdentityOptions>>(),
            NullLogger<SignInManager<IdentityUser>>.Instance, Schemes, GetRequiredService<IUserConfirmation<IdentityUser>>());
        IdentityLogs = new IdentitySecurityLogManager(SecurityLogs, Users, GetRequiredService<ICurrentPrincipalAccessor>(),
            GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>(), CurrentUser);
        Settings.GetOrNullAsync(Arg.Any<string>()).Returns("true");
        Schemes.GetAllSchemesAsync().Returns(Array.Empty<AuthenticationScheme>());
        Errors.Convert(Arg.Any<Exception>(), Arg.Any<Action<AbpExceptionHandlingOptions>>())
            .Returns(call => new RemoteServiceErrorInfo(call.Arg<Exception>().Message));
        var alertManager = Substitute.For<IAlertManager>();
        alertManager.Alerts.Returns(Alerts);
        ServiceOverrides[typeof(ISettingProvider)] = Settings;
        ServiceOverrides[typeof(ICurrentUser)] = CurrentUser;
        ServiceOverrides[typeof(IAlertManager)] = alertManager;
        ServiceOverrides[typeof(IModelStateValidator)] = new ModelStateValidator();
        PageServices = Substitute.For<IServiceProvider>();
        PageServices.GetService(Arg.Any<Type>()).Returns(call =>
            ServiceOverrides.TryGetValue(call.Arg<Type>(), out var value) ? value : ServiceProvider.GetService(call.Arg<Type>()));
    }

    protected T Page<T>(T page) where T : AccountPageModel
    {
        page.LazyServiceProvider = new AbpLazyServiceProvider(PageServices);
        page.AccountAppService = Account;
        page.UserManager = Users;
        page.SignInManager = SignIn;
        page.IdentitySecurityLogManager = IdentityLogs;
        page.IdentityOptions = GetRequiredService<IOptions<IdentityOptions>>();
        page.ExceptionToErrorInfoConverter = Errors;
        page.PageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext { RequestServices = PageServices },
            RouteData = new RouteData(),
            ActionDescriptor = new CompiledPageActionDescriptor { ViewEnginePath = "/Account/Login", RelativePath = "/Pages/Account/Login.cshtml" }
        };
        page.PageContext.RouteData.Values["page"] = "/Account/Login";
        page.Url = Substitute.For<IUrlHelper>();
        page.Url.ActionContext.Returns(page.PageContext);
        page.Url.IsLocalUrl(Arg.Any<string>()).Returns(call => call.Arg<string>()?.StartsWith("/", StringComparison.Ordinal) == true);
        return page;
    }

    protected async Task<IdentityUser> CreateUserAsync(string name = "web-user", string email = "web@example.com", bool external = false)
    {
        var user = new IdentityUser(Guid.NewGuid(), name, email) { IsExternal = external };
        (await Users.CreateAsync(user, "P@ssW0rd")).CheckErrors();
        await GetRequiredService<IUnitOfWorkManager>().Current.SaveChangesAsync();
        return user;
    }

    protected static ExternalLoginInfo ExternalInfo(string email = null)
    {
        var identity = new ClaimsIdentity("External");
        if (email != null)
        {
            identity.AddClaim(new Claim(ClaimTypes.Email, email));
        }
        return new ExternalLoginInfo(new ClaimsPrincipal(identity), "Provider", "external-key", "External provider");
    }
}
