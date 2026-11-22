using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Account.Settings;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Settings;
using Volo.Abp.Validation;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace SharpAbp.Abp.Account.Web.Pages.Account
{
    public class RegisterModel : AccountPageModel
    {

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrlHash { get; set; }

        [BindProperty]
        public PostInput Input { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool IsExternalLogin { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ExternalLoginAuthSchema { get; set; }

        protected SignInResult RegistrationSignInResult { get; set; }

        public RegisterModel(IAccountAppService accountAppService)
        {
            AccountAppService = accountAppService;
        }

        public virtual async Task<IActionResult> OnGetAsync()
        {
            await CheckSelfRegistrationAsync();
            await TrySetEmailAsync();
            return Page();
        }

        private async Task TrySetEmailAsync()
        {
            if (IsExternalLogin)
            {
                var externalLoginInfo = await SignInManager.GetExternalLoginInfoAsync();
                if (externalLoginInfo == null)
                {
                    return;
                }

                if (!externalLoginInfo.Principal.Identities.Any())
                {
                    return;
                }

                var identity = externalLoginInfo.Principal.Identities.First();
                var emailClaim = identity.FindFirst(ClaimTypes.Email);

                if (emailClaim == null)
                {
                    return;
                }

                Input = new PostInput { EmailAddress = emailClaim.Value };
            }
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await CheckSelfRegistrationAsync();
                RegistrationSignInResult = null;
                var originalPrincipal = SignInManager.Context.User;

                if (IsExternalLogin)
                {
                    var externalLoginInfo = await SignInManager.GetExternalLoginInfoAsync();
                    if (externalLoginInfo == null)
                    {
                        Logger.LogWarning("External login info is not available");
                        return RedirectToPage("./Login");
                    }

                    await RegisterExternalUserAsync(externalLoginInfo, Input.EmailAddress);
                }
                else
                {
                    await RegisterLocalUserAsync();
                }

                RegistrationSignInResult ??= !ReferenceEquals(originalPrincipal, SignInManager.Context.User) &&
                    SignInManager.IsSignedIn(SignInManager.Context.User)
                    ? SignInResult.Success
                    : SignInResult.Failed;

                if (RegistrationSignInResult.RequiresTwoFactor)
                {
                    return await TwoFactorLoginResultAsync();
                }

                if (!RegistrationSignInResult.Succeeded)
                {
                    Alerts.Warning(L[RegistrationSignInResult.IsLockedOut ? "UserLockedOutMessage" : "LoginIsNotAllowed"]);
                    return RedirectToPage("./Login", new { ReturnUrl, ReturnUrlHash });
                }

                return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
            }
            catch (BusinessException e)
            {
                Alerts.Danger(GetLocalizeExceptionMessage(e));
                return Page();
            }
        }

        protected virtual async Task RegisterLocalUserAsync()
        {
            ValidateModel();

            var userDto = await AccountAppService.RegisterAsync(
                new RegisterDto
                {
                    AppName = "MVC",
                    EmailAddress = Input.EmailAddress,
                    Password = Input.Password,
                    UserName = Input.UserName
                }
            );

            var user = await UserManager.GetByIdAsync(userDto.Id);
            await IdentityOptions.SetAsync();
            RegistrationSignInResult = await SignInManager.PasswordSignInAsync(user, Input.Password, isPersistent: true, lockoutOnFailure: true);
        }

        protected virtual async Task RegisterExternalUserAsync(ExternalLoginInfo externalLoginInfo, string emailAddress)
        {
            await IdentityOptions.SetAsync();

            var user = new IdentityUser(GuidGenerator.Create(), emailAddress, emailAddress, CurrentTenant.Id);

            (await UserManager.CreateAsync(user)).CheckErrors();
            (await UserManager.AddDefaultRolesAsync(user)).CheckErrors();

            var userLoginAlreadyExists = user.Logins.Any(x =>
                x.TenantId == user.TenantId &&
                x.LoginProvider == externalLoginInfo.LoginProvider &&
                x.ProviderKey == externalLoginInfo.ProviderKey);

            if (!userLoginAlreadyExists)
            {
                (await UserManager.AddLoginAsync(user, new UserLoginInfo(
                    externalLoginInfo.LoginProvider,
                    externalLoginInfo.ProviderKey,
                    externalLoginInfo.ProviderDisplayName
                ))).CheckErrors();
            }

            RegistrationSignInResult = await SignInManager.ExternalLoginSignInAsync(
                externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: true, bypassTwoFactor: false);
        }

        /// <summary>
        /// Override this method to continue registration with your application's 2FA flow.
        /// </summary>
        protected virtual Task<IActionResult> TwoFactorLoginResultAsync()
        {
            Alerts.Warning(L["LoginIsNotAllowed"]);
            return Task.FromResult<IActionResult>(RedirectToPage("./Login", new { ReturnUrl, ReturnUrlHash }));
        }

        protected virtual async Task CheckSelfRegistrationAsync()
        {
            if (!await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled) ||
                !await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin))
            {
                throw new UserFriendlyException(L["SelfRegistrationDisabledMessage"]);
            }
        }

        public class PostInput
        {
            [Required]
            [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxUserNameLength))]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
            public string EmailAddress { get; set; }

            [Required]
            [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
            [DataType(DataType.Password)]
            [DisableAuditing]
            public string Password { get; set; }
        }
    }
}
