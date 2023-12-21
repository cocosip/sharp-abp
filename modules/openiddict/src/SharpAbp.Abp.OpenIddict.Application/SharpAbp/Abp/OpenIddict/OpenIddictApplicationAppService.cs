using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.PermissionManagement;

namespace SharpAbp.Abp.OpenIddict
{
    [Authorize(OpenIddictPermissions.Applications.Default)]
    public class OpenIddictApplicationAppService : OpenIddictAppServiceBase, IOpenIddictApplicationAppService
    {
        protected IOpenIddictApplicationStoreResolver Resolver { get; }
        protected IAbpOpenIdApplicationStore OpenIdApplicationStore { get; }
        protected IAbpApplicationManager ApplicationManager { get; }
        protected IPermissionDataSeeder PermissionDataSeeder { get; }
        protected IPermissionManager PermissionManager { get; }
        protected IOpenIddictApplicationRepository OpenIdApplicationRepository { get; }
        protected IStringLocalizer<OpenIddictResponse> LL { get; }
        public OpenIddictApplicationAppService(
            IOpenIddictApplicationStoreResolver resolver,
            IAbpApplicationManager applicationManager,
            IPermissionDataSeeder permissionDataSeeder,
            IPermissionManager permissionManager,
            IOpenIddictApplicationRepository openIddictApplicationRepository,
            IStringLocalizer<OpenIddictResponse> ll)
        {
            Resolver = resolver;
            OpenIdApplicationStore = (resolver ?? throw new ArgumentNullException(nameof(resolver))).Get<OpenIddictApplicationModel>().As<IAbpOpenIdApplicationStore>();
            ApplicationManager = applicationManager;
            PermissionDataSeeder = permissionDataSeeder;
            PermissionManager = permissionManager;
            OpenIdApplicationRepository = openIddictApplicationRepository;
            LL = ll;
        }

        [Authorize(OpenIddictPermissions.Applications.Default)]
        public virtual async Task<OpenIddictApplicationDto> GetAsync(Guid id)
        {
            var application = await OpenIdApplicationRepository.GetAsync(id);
            return await ToApplicationDtoAsync(application.ToModel());
        }

        [Authorize(OpenIddictPermissions.Applications.Default)]
        public virtual async Task<OpenIddictApplicationDto> FindByClientIdAsync(string clientId)
        {
            Check.NotNullOrWhiteSpace(clientId, nameof(clientId));
            var application = await OpenIdApplicationRepository.FindByClientIdAsync(clientId);
            return await ToApplicationDtoAsync(application.ToModel());
        }

        [Authorize(OpenIddictPermissions.Applications.Default)]
        public virtual async Task<PagedResultDto<OpenIddictApplicationDto>> GetPagedListAsync(OpenIddictApplicationPagedRequestDto input)
        {
            var count = await OpenIdApplicationRepository.GetCountAsync(input.Filter);
            var applications = await OpenIdApplicationRepository.GetListAsync(input.Sorting, input.SkipCount, input.MaxResultCount, input.Filter);
            var applicationDtos = new List<OpenIddictApplicationDto>();
            foreach (var application in applications)
            {
                applicationDtos.Add(await ToApplicationDtoAsync(application.ToModel()));
            }
            return new PagedResultDto<OpenIddictApplicationDto>(count, applicationDtos);
        }

        [Authorize(OpenIddictPermissions.Applications.Default)]
        public virtual async Task<List<OpenIddictApplicationDto>> GetListAsync()
        {
            var applications = await OpenIdApplicationRepository.GetListAsync();
            var applicationDtos = new List<OpenIddictApplicationDto>();
            foreach (var application in applications)
            {
                applicationDtos.Add(await ToApplicationDtoAsync(application.ToModel()));
            }
            return applicationDtos;
        }

        [Authorize(OpenIddictPermissions.Applications.Create)]
        public virtual async Task<OpenIddictApplicationDto> CreateAsync(CreateOrUpdateOpenIddictApplicationDto input)
        {
            if (!string.IsNullOrEmpty(input.ClientSecret) && string.Equals(input.ClientType, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessException(LL["NoClientSecretCanBeSetForPublicApplications"]);
            }

            if (string.IsNullOrEmpty(input.ClientSecret) && string.Equals(input.ClientType, OpenIddictConstants.ClientTypes.Confidential, StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessException(LL["TheClientSecretIsRequiredForConfidentialApplications"]);
            }

            var model = await BuildModel(input);

            if (input.Permissions.Any())
            {
                await PermissionDataSeeder.SeedAsync(
                    ClientPermissionValueProvider.ProviderName,
                    input.ClientId,
                    input.Permissions,
                    null);
            }
            var descriptor = new AbpApplicationDescriptor()
            {
                LogoUri = input.LogoUri,
                ClientUri = input.ClientUri
            };

            await ApplicationManager.PopulateAsync(descriptor, model);
            var created = await ApplicationManager.CreateAsync(descriptor);
            return await ToApplicationDtoAsync(created.As<OpenIddictApplicationModel>());
        }

        [Authorize(OpenIddictPermissions.Applications.Update)]
        public virtual async Task<OpenIddictApplicationDto> UpdateAsync(Guid id, CreateOrUpdateOpenIddictApplicationDto input)
        {
            if (!string.IsNullOrEmpty(input.ClientSecret) && string.Equals(input.ClientType, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessException(LL["NoClientSecretCanBeSetForPublicApplications"]);
            }

            // if (string.IsNullOrEmpty(input.ClientSecret) && string.Equals(input.Type, OpenIddictConstants.ClientTypes.Confidential, StringComparison.OrdinalIgnoreCase))
            // {
            //     throw new BusinessException(LL["TheClientSecretIsRequiredForConfidentialApplications"]);
            // }


            var model = (await ApplicationManager.FindByIdAsync(id.ToString("D"))).As<OpenIddictApplicationModel>();
            var buildModel = await BuildModel(input);
            if (buildModel.ClientSecret.IsNullOrWhiteSpace())
            {
                buildModel.ClientSecret = model.ClientSecret;
            }

            if (input.Permissions.Any())
            {
                await PermissionManager.DeleteAsync(ClientPermissionValueProvider.ProviderName, model.ClientId);
                await PermissionDataSeeder.SeedAsync(
                    ClientPermissionValueProvider.ProviderName,
                    input.ClientId,
                    input.Permissions,
                    null);
            }

            var descriptor = new AbpApplicationDescriptor()
            {
                LogoUri = input.LogoUri,
                ClientUri = input.ClientUri
            };

            await ApplicationManager.PopulateAsync(descriptor, buildModel);
            await ApplicationManager.UpdateAsync(model, descriptor);

            var updated = (await ApplicationManager.FindByIdAsync(id.ToString("D"))).As<OpenIddictApplicationModel>();
            return await ToApplicationDtoAsync(updated);
        }

        [Authorize(OpenIddictPermissions.Applications.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await OpenIdApplicationRepository.DeleteAsync(id);
        }

        protected virtual async Task<OpenIddictApplicationModel> BuildModel(CreateOrUpdateOpenIddictApplicationDto input)
        {
            var model = new OpenIddictApplicationModel()
            {
                ClientId = input.ClientId,
                ClientType = input.ClientType,
                ClientSecret = input.ClientSecret,
                ConsentType = input.ConsentType,
                DisplayName = input.DisplayName,
                ClientUri = input.ClientUri,
                LogoUri = input.LogoUri,
            };

            var displayNames = new Dictionary<CultureInfo, string>(input.DisplayNames.Select(x => new KeyValuePair<CultureInfo, string>(new CultureInfo(x.Key), x.Value)));
            await OpenIdApplicationStore.SetDisplayNamesAsync(model, displayNames.ToImmutableDictionary(), CancellationToken.None);

            await OpenIdApplicationStore.SetRequirementsAsync(model, input.Requirements.ToImmutableArray(), CancellationToken.None);

            //RedirectUris
            var redirectUris = new List<string>();
            foreach (var redirectUri in input.RedirectUris)
            {
                if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
                {
                    throw new BusinessException(LL["InvalidRedirectUri", redirectUri]);
                }
                redirectUris.Add(uri.ToString());
            }

            await OpenIdApplicationStore.SetRedirectUrisAsync(model, redirectUris.ToImmutableArray(), CancellationToken.None);
            var firstRedirectUri = redirectUris.FirstOrDefault();

            var postLogoutRedirectUris = new List<string>();
            foreach (var postLogoutRedirectUri in input.PostLogoutRedirectUris)
            {
                if (!Uri.TryCreate(postLogoutRedirectUri, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
                {
                    throw new BusinessException(LL["InvalidRedirectUri", postLogoutRedirectUri]);
                }
                postLogoutRedirectUris.Add(uri.ToString());
            }
            await OpenIdApplicationStore.SetPostLogoutRedirectUrisAsync(model, postLogoutRedirectUris.ToImmutableArray(), CancellationToken.None);
            var firstPostLogoutRedirectUri = postLogoutRedirectUris.FirstOrDefault();

            var permissions = new List<string>();
            if (new[] { OpenIddictConstants.GrantTypes.AuthorizationCode, OpenIddictConstants.GrantTypes.Implicit }.All(input.GrantTypes.Contains))
            {
                permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken);

                if (string.Equals(input.ClientType, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
                {
                    permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken);
                    permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeToken);
                }
            }

            if (!firstRedirectUri.IsNullOrWhiteSpace() || !firstPostLogoutRedirectUri.IsNullOrWhiteSpace())
            {
                permissions.Add(OpenIddictConstants.Permissions.Endpoints.Logout);
            }

            foreach (var grantType in input.GrantTypes)
            {
                if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode)
                {
                    permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
                    permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
                }

                if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode || grantType == OpenIddictConstants.GrantTypes.Implicit)
                {
                    permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
                }

                if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode ||
                    grantType == OpenIddictConstants.GrantTypes.ClientCredentials ||
                    grantType == OpenIddictConstants.GrantTypes.Password ||
                    grantType == OpenIddictConstants.GrantTypes.RefreshToken ||
                    grantType == OpenIddictConstants.GrantTypes.DeviceCode)
                {
                    permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                    permissions.Add(OpenIddictConstants.Permissions.Endpoints.Revocation);
                    permissions.Add(OpenIddictConstants.Permissions.Endpoints.Introspection);
                }

                if (grantType == OpenIddictConstants.GrantTypes.ClientCredentials)
                {
                    permissions.Add(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials);
                }

                if (grantType == OpenIddictConstants.GrantTypes.Implicit)
                {
                    permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Implicit);
                }

                if (grantType == OpenIddictConstants.GrantTypes.Password)
                {
                    permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);
                }

                if (grantType == OpenIddictConstants.GrantTypes.RefreshToken)
                {
                    permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
                }

                if (grantType == OpenIddictConstants.GrantTypes.DeviceCode)
                {
                    permissions.Add(OpenIddictConstants.Permissions.GrantTypes.DeviceCode);
                    permissions.Add(OpenIddictConstants.Permissions.Endpoints.Device);
                }

                if (grantType == OpenIddictConstants.GrantTypes.Implicit)
                {
                    permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdToken);
                    if (string.Equals(input.ClientType, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
                    {
                        permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken);
                        permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Token);
                    }
                }
            }

            var buildInScopes = GetAllScopes();

            foreach (var scope in input.Scopes)
            {
                if (buildInScopes.Contains(scope))
                {
                    permissions.Add(scope);
                }
                else
                {
                    permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + scope);
                }
            }

            await OpenIdApplicationStore.SetPermissionsAsync(model, permissions.ToImmutableArray(), CancellationToken.None);
            return model;
        }

        protected virtual async Task<OpenIddictApplicationDto> ToApplicationDtoAsync(OpenIddictApplicationModel model)
        {

            var dto = new OpenIddictApplicationDto()
            {
                Id = model.Id,
                ClientId = model.ClientId,
                ClientSecret = model.ClientSecret,
                ConsentType = model.ConsentType,
                DisplayName = model.DisplayName,
                ClientType = model.ClientType,
                ClientUri = model.ClientUri,
                LogoUri = model.LogoUri,
            };

            foreach (var extraProperty in model.ExtraProperties)
            {
                dto.ExtraProperties.Add(extraProperty.Key, extraProperty.Value);
            }

            dto.Properties = new Dictionary<string, JsonElement>(await ApplicationManager.GetPropertiesAsync(model));

            var displayNames = await ApplicationManager.GetDisplayNamesAsync(model);
            dto.DisplayNames = new Dictionary<string, string>(displayNames.Select(x => new KeyValuePair<string, string>(x.Key.Name, x.Value)));

            var redirectUris = await ApplicationManager.GetRedirectUrisAsync(model);
            dto.RedirectUris = redirectUris.ToList();

            var postLogoutRedirectUris = await ApplicationManager.GetPostLogoutRedirectUrisAsync(model);
            dto.PostLogoutRedirectUris = postLogoutRedirectUris.ToList();

            var requirements = await ApplicationManager.GetRequirementsAsync(model);
            dto.Requirements = requirements.ToList();

            var permissions = await ApplicationManager.GetPermissionsAsync(model);
            // var commonScopes = GetAllScopes();

            foreach (var permission in permissions)
            {
                if (permission.StartsWith(OpenIddictConstants.Permissions.Prefixes.Scope))
                {
                    dto.Scopes.Add(permission.RemovePreFix(OpenIddictConstants.Permissions.Prefixes.Scope));
                }

                if (permission.StartsWith(OpenIddictConstants.Permissions.Prefixes.GrantType))
                {
                    dto.GrantTypes.Add(permission.RemovePreFix(OpenIddictConstants.Permissions.Prefixes.GrantType));
                }
            }

            return dto;
        }

        protected virtual List<string> GetAllGrantTypes()
        {
            var grantTypes = new List<string>()
            {
                OpenIddictConstants.GrantTypes.AuthorizationCode,
                OpenIddictConstants.GrantTypes.ClientCredentials,
                OpenIddictConstants.GrantTypes.DeviceCode,
                OpenIddictConstants.GrantTypes.Implicit,
                OpenIddictConstants.GrantTypes.Password,
                OpenIddictConstants.GrantTypes.RefreshToken
            };

            return grantTypes;
        }

        protected virtual List<string> GetAllScopes()
        {
            var scopes = new List<string>()
            {
                OpenIddictConstants.Permissions.Scopes.Address,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Phone,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
            };
            return scopes;
        }


    }
}
