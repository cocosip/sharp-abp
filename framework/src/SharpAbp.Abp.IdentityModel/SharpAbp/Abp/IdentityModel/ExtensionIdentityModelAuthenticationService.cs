using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.IdentityModel;

namespace SharpAbp.Abp.IdentityModel
{
    public class ExtensionIdentityModelAuthenticationService : IExtensionIdentityModelAuthenticationService, ITransientDependency
    {
        protected ILogger Logger { get; set; }
        protected AbpIdentityClientOptions ClientOptions { get; }
        protected IIdentityModelAuthenticationService IdentityModelAuthenticationService { get; }

        public ExtensionIdentityModelAuthenticationService(
            ILogger<ExtensionIdentityModelAuthenticationService> logger,
            IOptions<AbpIdentityClientOptions> options,
            IIdentityModelAuthenticationService identityModelAuthenticationService)
        {
            Logger = logger;
            ClientOptions = options.Value;
            IdentityModelAuthenticationService = identityModelAuthenticationService;
        }

        /// <summary>
        /// Get user accessToken
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <param name="identityClientName"></param>
        /// <returns></returns>
        public virtual async Task<string?> GetUserAccessTokenAsync(
            string userName,
            string userPassword,
            string? identityClientName = null)
        {
            var configuration = GetClientConfiguration(userName, userPassword, identityClientName);
            if (configuration == null)
            {
                Logger.LogWarning("Could not find {IdentityClientConfiguration} for client name '{IdentityClientName}'. Either define a configuration for this client or set a default configuration.",
                    nameof(IdentityClientConfiguration), identityClientName ?? "null");
                return null;
            }

            try
            {
                return await IdentityModelAuthenticationService.GetAccessTokenAsync(configuration);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to get access token for user '{UserName}' with client '{IdentityClientName}'.",
                    userName, identityClientName ?? "default");
                throw;
            }
        }

        /// <summary>
        /// ExternalCredentials
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <param name="identityClientName"></param>
        /// <returns></returns>
        public virtual async Task<string?> GetExternalCredentialsAccessTokenAsync(
            string loginProvider,
            string providerKey,
            string? identityClientName = null)
        {
            var configuration = GetClientConfiguration(loginProvider, providerKey, identityClientName);
            if (configuration == null)
            {
                Logger.LogWarning("Could not find {IdentityClientConfiguration} for client name '{IdentityClientName}'. Either define a configuration for this client or set a default configuration.",
                    nameof(IdentityClientConfiguration), identityClientName ?? "null");
                return null;
            }

            try
            {
                return await IdentityModelAuthenticationService.GetAccessTokenAsync(configuration);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to get access token for external credentials. Login provider: '{LoginProvider}', Provider key: '{ProviderKey}', Client name: '{IdentityClientName}'.",
                    loginProvider, providerKey, identityClientName ?? "default");
                throw;
            }
        }

        protected virtual IdentityClientConfiguration GetClientConfiguration(
            string userName,
            string userPassword,
            string? identityClientName = null)
        {
            IdentityClientConfiguration? identityClientConfiguration;
            if (identityClientName.IsNullOrWhiteSpace())
            {
                identityClientConfiguration = ClientOptions.IdentityClients.Default;
            }
            else
            {
                identityClientConfiguration = ClientOptions.IdentityClients.GetOrDefault(identityClientName) ??
                    ClientOptions.IdentityClients.Default;
            }

            var configuration = new IdentityClientConfiguration(
                identityClientConfiguration?.Authority!,
                identityClientConfiguration?.Scope!,
                identityClientConfiguration?.ClientId!,
                identityClientConfiguration?.ClientSecret!,
                identityClientConfiguration?.GrantType!,
                userName,
                userPassword,
                identityClientConfiguration?.RequireHttps ?? false,
                identityClientConfiguration?.CacheAbsoluteExpiration ?? 1800);

            return configuration;
        }

    }
}
