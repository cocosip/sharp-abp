using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.IdentityServer.AspNetIdentity;
using Volo.Abp.IdentityServer.Localization;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace SharpAbp.Abp.IdentityServer.Extensions
{
    public class ExternalCredentialsGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => GrantTypeConstants.ExternalCredentials;
        protected ILogger Logger { get; }

        protected AbpIdentityOptions AbpIdentityOptions { get; }
        protected IOptions<IdentityOptions> IdentityOptions { get; }
        protected IStringLocalizer<AbpIdentityServerResource> Localizer { get; }
        protected SignInManager<IdentityUser> SignInManager { get; }
        protected IdentityUserManager UserManager { get; }
        protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }
        public ExternalCredentialsGrantValidator(
            ILogger<ExternalCredentialsGrantValidator> logger,
            IOptions<AbpIdentityOptions> abpIdentityOptions,
            IOptions<IdentityOptions> identityOptions,
            IStringLocalizer<AbpIdentityServerResource> localizer,
            SignInManager<IdentityUser> signInManager,
            IdentityUserManager userManager,
            IdentitySecurityLogManager identitySecurityLogManager)
        {
            Logger = logger;
            AbpIdentityOptions = abpIdentityOptions.Value;
            IdentityOptions = identityOptions;
            Localizer = localizer;

            SignInManager = signInManager;
            UserManager = userManager;
            IdentitySecurityLogManager = identitySecurityLogManager;
        }

        [UnitOfWork]
        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var loginProvider = context.GetLoginProvider();
            if (loginProvider.IsNullOrWhiteSpace())
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid LoginProvider");
                return;
            }

            var providerKey = context.GetProviderKey();
            if (providerKey.IsNullOrWhiteSpace())
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid ProviderKey");
                return;
            }

            var user = await UserManager.FindByLoginAsync(loginProvider, providerKey);
            string errorDescription;
            if (user != null)
            {
                await IdentityOptions.SetAsync();
                var result = await SignInManager.ExternalLoginSignInAsync(loginProvider, providerKey, false);
                if (result.Succeeded)
                {
                    if (await IsTfaEnabledAsync(user))
                    {
                        await HandleTwoFactorLoginAsync(context, user);
                    }
                    else
                    {
                        await SetSuccessResultAsync(context, user);
                    }
                    return;
                }

                if (result.IsLockedOut)
                {
                    Logger.LogInformation("Authentication failed for username: {0}, reason: locked out", user.UserName);
                    errorDescription = Localizer["UserLockedOut"];
                }
                else if (result.IsNotAllowed)
                {
                    Logger.LogInformation("Authentication failed for username: {0}, reason: not allowed", user.UserName);
                    errorDescription = Localizer["LoginIsNotAllowed"];
                }
                else
                {
                    Logger.LogInformation("Authentication failed for provider {0}, reason: invalid credentials", context.GetLoginProvider());
                    errorDescription = Localizer["InvalidUserNameOrPassword"];
                }

                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                {
                    Identity = IdentityServerSecurityLogIdentityConsts.IdentityServer,
                    Action = result.ToIdentitySecurityLogAction(),
                    UserName = $"{context.GetProviderKey()}({context.GetLoginProvider()})",
                    ClientId = await FindClientIdAsync(context)
                });

            }
            else
            {
                //User is null
                Logger.LogInformation("No user found matching providerKey: {0}(1) .", loginProvider, providerKey);
                errorDescription = Localizer["InvalidUsername"];

                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                {
                    Identity = IdentityServerSecurityLogIdentityConsts.IdentityServer,
                    Action = IdentityServerSecurityLogActionConsts.LoginInvalidUserName,
                    UserName = $"{providerKey}({loginProvider})",
                    ClientId = await FindClientIdAsync(context)
                });
            }

            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, errorDescription);
        }


        protected virtual async Task<bool> IsTfaEnabledAsync(IdentityUser user)
           => UserManager.SupportsUserTwoFactor &&
              await UserManager.GetTwoFactorEnabledAsync(user) &&
              (await UserManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;

        protected virtual Task<string> FindClientIdAsync(ExtensionGrantValidationContext context)
        {
            return Task.FromResult(context.Request?.Client?.ClientId);
        }

        protected virtual Task AddCustomClaimsAsync(List<Claim> customClaims, IdentityUser user, ExtensionGrantValidationContext context)
        {
            if (user.TenantId.HasValue)
            {
                customClaims.Add(
                    new Claim(
                        AbpClaimTypes.TenantId,
                        user.TenantId?.ToString()
                    )
                );
            }

            return Task.CompletedTask;
        }

        protected virtual async Task SetSuccessResultAsync(ExtensionGrantValidationContext context, IdentityUser user)
        {
            var sub = await UserManager.GetUserIdAsync(user);

            Logger.LogInformation("Credentials validated for provider: {0}", context.Request.Raw[ExternalCredentialsParameterConstants.ProviderKey]);

            var additionalClaims = new List<Claim>();

            await AddCustomClaimsAsync(additionalClaims, user, context);

            context.Result = new GrantValidationResult(
                sub,
                GrantTypeConstants.ExternalCredentials,
                additionalClaims.ToArray()
            );

            await IdentitySecurityLogManager.SaveAsync(
                new IdentitySecurityLogContext
                {
                    Identity = IdentityServerSecurityLogIdentityConsts.IdentityServer,
                    Action = IdentityServerSecurityLogActionConsts.LoginSucceeded,
                    UserName = $"{context.GetProviderKey()}({context.GetLoginProvider()})",
                    ClientId = await FindClientIdAsync(context)
                }
            );
        }

        protected virtual async Task HandleTwoFactorLoginAsync(ExtensionGrantValidationContext context, IdentityUser user)
        {
            var twoFactorProvider = context.Request?.Raw?["TwoFactorProvider"];
            var twoFactorCode = context.Request?.Raw?["TwoFactorCode"];
            if (!twoFactorProvider.IsNullOrWhiteSpace() && !twoFactorCode.IsNullOrWhiteSpace())
            {
                var providers = await UserManager.GetValidTwoFactorProvidersAsync(user);
                if (providers.Contains(twoFactorProvider) && await UserManager.VerifyTwoFactorTokenAsync(user, twoFactorProvider, twoFactorCode))
                {
                    await SetSuccessResultAsync(context, user);
                    return;
                }

                Logger.LogInformation("Authentication failed for providerKey: {0}({1}), reason: InvalidAuthenticatorCode", context.GetProviderKey(), context.GetLoginProvider());
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, Localizer["InvalidAuthenticatorCode"]);
            }
            else
            {
                Logger.LogInformation("Authentication failed for providerKey: {0}({1}), reason: RequiresTwoFactor", context.GetProviderKey(), context.GetLoginProvider());
                var twoFactorToken = await UserManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, nameof(SignInResult.RequiresTwoFactor));
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, nameof(SignInResult.RequiresTwoFactor),
                    new Dictionary<string, object>()
                    {
                        {"userId", user.Id},
                        {"twoFactorToken", twoFactorToken}
                    });

                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                {
                    Identity = IdentityServerSecurityLogIdentityConsts.IdentityServer,
                    Action = IdentityServerSecurityLogActionConsts.LoginRequiresTwoFactor,
                    UserName = $"{context.GetProviderKey()}({context.GetLoginProvider()})",
                    ClientId = await FindClientIdAsync(context)
                });
            }
        }


    }
}
