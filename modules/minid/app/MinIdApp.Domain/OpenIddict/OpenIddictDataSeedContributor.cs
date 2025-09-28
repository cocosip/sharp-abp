using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using OpenIddict.Abstractions;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace MinIdApp.OpenIddict
{
    public class OpenIddictDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        public static string OpenIdConnectExtensionGrant = "OpenIdConnectExtensionGrant";

        private readonly IConfiguration _configuration;
        private readonly IAbpApplicationManager _applicationManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly IPermissionDataSeeder _permissionDataSeeder;
        private readonly IStringLocalizer<OpenIddictResponse> L;

        public OpenIddictDataSeedContributor(
            IConfiguration configuration,
            IAbpApplicationManager applicationManager,
            IOpenIddictScopeManager scopeManager,
            IPermissionDataSeeder permissionDataSeeder,
            IStringLocalizer<OpenIddictResponse> l)
        {
            _configuration = configuration;
            _applicationManager = applicationManager;
            _scopeManager = scopeManager;
            _permissionDataSeeder = permissionDataSeeder;
            L = l;
        }

        [UnitOfWork]
        public virtual async Task SeedAsync(DataSeedContext context)
        {
            await CreateScopesAsync();
            await CreateApplicationsAsync();
        }

        private async Task CreateScopesAsync()
        {
            if (await _scopeManager.FindByNameAsync("Hidos") == null)
            {
                await _scopeManager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    Name = "MinIdApp",
                    DisplayName = "MinIdApp API",
                    Resources =
                    {
                        "MinIdApp"
                    }
                });
            }
        }

        private async Task CreateApplicationsAsync()
        {
            var commonScopes = new List<string>{
                OpenIddictConstants.Permissions.Scopes.Address,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Phone,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                "Hidos"
            };

            var configurationSection = _configuration.GetSection("OpenIddict:Applications");

            //Web Client
            var webClientId = configurationSection["Hidos_Web:ClientId"];
            if (!webClientId.IsNullOrWhiteSpace())
            {
                var webClientRootUrl = configurationSection["Hidos_Web:RootUrl"].EnsureEndsWith('/');

                /* Hidos_Web client is only needed if you created a tiered
                 * solution. Otherwise, you can delete this client. */
                await CreateApplicationAsync(
                    name: webClientId,
                    clientType: OpenIddictConstants.ClientTypes.Confidential,
                    consentType: OpenIddictConstants.ConsentTypes.Implicit,
                    displayName: "Web Application",
                    secret: configurationSection["Hidos_Web:ClientSecret"] ?? "1q2w3e*",
                    grantTypes:
                    [
                        OpenIddictConstants.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.GrantTypes.Implicit
                    ],
                    scopes: commonScopes,
                    redirectUri: $"{webClientRootUrl}signin-oidc",
                    clientUri: webClientRootUrl,
                    postLogoutRedirectUri: $"{webClientRootUrl}signout-callback-oidc"
                );
            }

            //Console Test / Angular Client
            var consoleAndAngularClientId = configurationSection["Hidos_App:ClientId"];
            if (!consoleAndAngularClientId.IsNullOrWhiteSpace())
            {
                var consoleAndAngularClientRootUrl = configurationSection["Hidos_App:RootUrl"]?.TrimEnd('/');
                await CreateApplicationAsync(
                    name: consoleAndAngularClientId,
                    clientType: OpenIddictConstants.ClientTypes.Public,
                    consentType: OpenIddictConstants.ConsentTypes.Implicit,
                    displayName: "Console Test / Angular Application",
                    secret: null,
                    grantTypes:
                    [
                        OpenIddictConstants.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.GrantTypes.Password,
                        OpenIddictConstants.GrantTypes.ClientCredentials,
                        OpenIddictConstants.GrantTypes.RefreshToken,
                    ],
                    scopes: commonScopes,
                    redirectUri: consoleAndAngularClientRootUrl,
                    clientUri: consoleAndAngularClientRootUrl,
                    postLogoutRedirectUri: consoleAndAngularClientRootUrl
                );
            }

            // Blazor Client
            var blazorClientId = configurationSection["Hidos_Blazor:ClientId"];
            if (!blazorClientId.IsNullOrWhiteSpace())
            {
                var blazorRootUrl = configurationSection["Hidos_Blazor:RootUrl"].TrimEnd('/');

                await CreateApplicationAsync(
                    name: blazorClientId,
                    clientType: OpenIddictConstants.ClientTypes.Public,
                    consentType: OpenIddictConstants.ConsentTypes.Implicit,
                    displayName: "Blazor Application",
                    secret: null,
                    grantTypes: new List<string>
                    {
                        OpenIddictConstants.GrantTypes.AuthorizationCode,
                    },
                    scopes: commonScopes,
                    redirectUri: $"{blazorRootUrl}/authentication/login-callback",
                    clientUri: blazorRootUrl,
                    postLogoutRedirectUri: $"{blazorRootUrl}/authentication/logout-callback"
                );
            }

            // Blazor Server Tiered Client
            var blazorServerTieredClientId = configurationSection["Hidos_BlazorServerTiered:ClientId"];
            if (!blazorServerTieredClientId.IsNullOrWhiteSpace())
            {
                var blazorServerTieredRootUrl = configurationSection["Hidos_BlazorServerTiered:RootUrl"].EnsureEndsWith('/');

                await CreateApplicationAsync(
                    name: blazorServerTieredClientId,
                    clientType: OpenIddictConstants.ClientTypes.Confidential,
                    consentType: OpenIddictConstants.ConsentTypes.Implicit,
                    displayName: "Blazor Server Application",
                    secret: configurationSection["Hidos_BlazorServerTiered:ClientSecret"] ?? "1q2w3e*",
                    grantTypes:
                    [
                        OpenIddictConstants.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.GrantTypes.Implicit
                    ],
                    scopes: commonScopes,
                    redirectUri: $"{blazorServerTieredRootUrl}signin-oidc",
                    clientUri: blazorServerTieredRootUrl,
                    postLogoutRedirectUri: $"{blazorServerTieredRootUrl}signout-callback-oidc"
                );
            }

            // Swagger Client
            var swaggerClientId = configurationSection["Hidos_Swagger:ClientId"];
            if (!swaggerClientId.IsNullOrWhiteSpace())
            {
                var swaggerRootUrl = configurationSection["Hidos_Swagger:RootUrl"].TrimEnd('/');

                await CreateApplicationAsync(
                    name: swaggerClientId,
                    clientType: OpenIddictConstants.ClientTypes.Public,
                    consentType: OpenIddictConstants.ConsentTypes.Implicit,
                    displayName: "Swagger Application",
                    secret: null,
                    grantTypes:
                    [
                         OpenIddictConstants.GrantTypes.AuthorizationCode,
                    ],
                    scopes: commonScopes,
                    redirectUri: $"{swaggerRootUrl}/swagger/oauth2-redirect.html",
                    clientUri: swaggerRootUrl
                );
            }
        }

        private async Task CreateApplicationAsync(
            [NotNull] string name,
            [NotNull] string clientType,
            [NotNull] string consentType,
            string displayName,
            string secret,
            List<string> grantTypes,
            List<string> scopes,
            string clientUri = null,
            string redirectUri = null,
            string postLogoutRedirectUri = null,
            List<string> permissions = null)
        {
            if (!string.IsNullOrEmpty(secret) && string.Equals(clientType, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessException(L["NoClientSecretCanBeSetForPublicApplications"]);
            }

            if (string.IsNullOrEmpty(secret) && string.Equals(clientType, OpenIddictConstants.ClientTypes.Confidential, StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessException(L["TheClientSecretIsRequiredForConfidentialApplications"]);
            }

            //if (!string.IsNullOrEmpty(name) && await _applicationManager.FindByClientIdAsync(name) != null)
            //{
            //    return;
            //    //throw new BusinessException(L["TheClientIdentifierIsAlreadyTakenByAnotherApplication"]);
            //}

            var client = await _applicationManager.FindByClientIdAsync(name);

            if (client == null)
            {
                var application = new AbpApplicationDescriptor
                {
                    ClientId = name,
                    ClientType = clientType,
                    ClientSecret = secret,
                    ConsentType = consentType,
                    DisplayName = displayName,
                    ClientUri = clientUri,
                };

                Check.NotNullOrEmpty(grantTypes, nameof(grantTypes));
                Check.NotNullOrEmpty(scopes, nameof(scopes));

                if (new[] { OpenIddictConstants.GrantTypes.AuthorizationCode, OpenIddictConstants.GrantTypes.Implicit }.All(grantTypes.Contains))
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken);

                    if (string.Equals(clientType, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
                    {
                        application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken);
                        application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeToken);
                    }
                }

                if (!redirectUri.IsNullOrWhiteSpace() || !postLogoutRedirectUri.IsNullOrWhiteSpace())
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.EndSession);
                }

                foreach (var grantType in grantTypes)
                {
                    if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode)
                    {
                        application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
                        application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
                    }

                    if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode || grantType == OpenIddictConstants.GrantTypes.Implicit)
                    {
                        application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
                    }

                    if (grantType == OpenIddictConstants.GrantTypes.ClientCredentials)
                    {
                        application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials);
                    }

                    if (grantType == OpenIddictConstants.GrantTypes.Implicit)
                    {
                        application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Implicit);
                    }

                    if (grantType == OpenIddictConstants.GrantTypes.Password)
                    {
                        application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);
                    }

                    if (grantType == OpenIddictConstants.GrantTypes.RefreshToken)
                    {
                        application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
                    }

                    if (grantType == OpenIddictConstants.GrantTypes.DeviceCode)
                    {
                        application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.DeviceCode);
                        application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.DeviceAuthorization);
                    }

                    if (grantType == OpenIddictConstants.GrantTypes.Implicit)
                    {
                        application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdToken);
                        if (string.Equals(clientType, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
                        {
                            application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken);
                            application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Token);
                        }
                    }
                }

                var buildInScopes = new[]
                {
                    OpenIddictConstants.Permissions.Scopes.Address,
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Phone,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles
                };

                foreach (var scope in scopes)
                {
                    if (buildInScopes.Contains(scope))
                    {
                        application.Permissions.Add(scope);
                    }
                    else
                    {
                        application.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + scope);
                    }
                }

                if (redirectUri != null)
                {
                    if (!redirectUri.IsNullOrEmpty())
                    {
                        if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
                        {
                            throw new BusinessException(L["InvalidRedirectUri", redirectUri]);
                        }

                        if (application.RedirectUris.All(x => x != uri))
                        {
                            application.RedirectUris.Add(uri);
                        }
                    }
                }

                if (postLogoutRedirectUri != null)
                {
                    if (!postLogoutRedirectUri.IsNullOrEmpty())
                    {
                        if (!Uri.TryCreate(postLogoutRedirectUri, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
                        {
                            throw new BusinessException(L["InvalidPostLogoutRedirectUri", postLogoutRedirectUri]);
                        }

                        if (application.PostLogoutRedirectUris.All(x => x != uri))
                        {
                            application.PostLogoutRedirectUris.Add(uri);
                        }
                    }
                }

                if (permissions != null)
                {
                    await _permissionDataSeeder.SeedAsync(
                        ClientPermissionValueProvider.ProviderName,
                        name,
                        permissions,
                        null
                    );
                }

                await _applicationManager.CreateAsync(application);
            }
        }
    }
}