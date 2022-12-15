using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.PermissionManagement;
using static System.Net.Mime.MediaTypeNames;

namespace SharpAbp.Abp.OpenIddict
{
    [Authorize(OpenIddictPermissions.Applications.Default)]
    public class OpenIddictApplicationAppService : OpenIddictAppServiceBase, IOpenIddictApplicationAppService
    {
        protected IAbpApplicationManager ApplicationManager { get; }
        protected IPermissionDataSeeder PermissionDataSeeder { get; }
        protected IPermissionManager PermissionManager { get; }
        public OpenIddictApplicationAppService(
            IAbpApplicationManager applicationManager,
            IPermissionDataSeeder permissionDataSeeder,
            IPermissionManager permissionManager)
        {
            ApplicationManager = applicationManager;
            PermissionDataSeeder = permissionDataSeeder;
            PermissionManager = permissionManager;
        }

        [Authorize(OpenIddictPermissions.Applications.Default)]
        public virtual async Task<OpenIddictApplicationDto> GetAsync(Guid id)
        {
            var application = await ApplicationManager.FindByIdAsync(id.ToString("D"));
            return await ToApplicationDtoAsync(application.As<OpenIddictApplicationModel>());
        }

        [Authorize(OpenIddictPermissions.Applications.Default)]
        public virtual async Task<OpenIddictApplicationDto> FindByClientIdAsync(string clientId)
        {
            var application = await ApplicationManager.FindByClientIdAsync(clientId);
            return await ToApplicationDtoAsync(application.As<OpenIddictApplicationModel>());
        }

        [Authorize(OpenIddictPermissions.Applications.Default)]
        public virtual async Task<PagedResultDto<OpenIddictApplicationDto>> GetPagedListAsync(PagedAndSortedResultRequestDto input)
        {
            var count = await ApplicationManager.CountAsync();
            var applications = ApplicationManager.ListAsync(input.MaxResultCount, input.SkipCount);

            var applicationDtos = new List<OpenIddictApplicationDto>();
            await foreach (var application in applications)
            {
                applicationDtos.Add(await ToApplicationDtoAsync(application.As<OpenIddictApplicationModel>()));
            }

            return new PagedResultDto<OpenIddictApplicationDto>(count, applicationDtos);
        }

        [Authorize(OpenIddictPermissions.Applications.Default)]
        public virtual async Task<List<OpenIddictApplicationDto>> GetListAsync()
        {
            var applications = ApplicationManager.ListAsync(int.MaxValue, 0);
            var applicationDtos = new List<OpenIddictApplicationDto>();
            await foreach (var application in applications)
            {
                applicationDtos.Add(await ToApplicationDtoAsync(application.As<OpenIddictApplicationModel>()));
            }
            return applicationDtos;
        }

        [Authorize(OpenIddictPermissions.Applications.Create)]
        public virtual async Task<OpenIddictApplicationDto> CreateAsync(CreateOrUpdateOpenIddictApplicationDto input)
        {
            var application = BuildApplicationDescriptor(input);

            if (input.Permissions.Any())
            {
                await PermissionDataSeeder.SeedAsync(
                    ClientPermissionValueProvider.ProviderName,
                    input.ClientId,
                    input.Permissions,
                    null);
            }

            var created = await ApplicationManager.CreateAsync(application);
            return await ToApplicationDtoAsync(created.As<OpenIddictApplicationModel>());
        }

        [Authorize(OpenIddictPermissions.Applications.Update)]
        public virtual async Task<OpenIddictApplicationDto> UpdateAsync(Guid id, CreateOrUpdateOpenIddictApplicationDto input)
        {
            var model = (await ApplicationManager.FindByIdAsync(id.ToString("D"))).As<OpenIddictApplicationModel>();
            var application = BuildApplicationDescriptor(input);

            await ApplicationManager.PopulateAsync(model, application);

            if (input.Permissions.Any())
            {
                await PermissionManager.DeleteAsync(ClientPermissionValueProvider.ProviderName, model.As<OpenIddictApplicationModel>().ClientId);
                await PermissionDataSeeder.SeedAsync(
                    ClientPermissionValueProvider.ProviderName,
                    input.ClientId,
                    input.Permissions,
                    null);
            }

            await ApplicationManager.UpdateAsync(model);
            model = (await ApplicationManager.FindByIdAsync(id.ToString("D"))).As<OpenIddictApplicationModel>();

            return await ToApplicationDtoAsync(model);
        }

        [Authorize(OpenIddictPermissions.Applications.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            var model = await ApplicationManager.FindByIdAsync(id.ToString("D"));
            await ApplicationManager.DeleteAsync(model);
        }

        protected virtual AbpApplicationDescriptor BuildApplicationDescriptor(CreateOrUpdateOpenIddictApplicationDto input)
        {
            var application = new AbpApplicationDescriptor()
            {
                ClientId = input.ClientId,
                Type = input.Type,
                ClientSecret = input.ClientSecret,
                ConsentType = input.ConsentType,
                DisplayName = input.DisplayName,
                ClientUri = input.ClientUri,
                LogoUri = input.LogoUri,
            };

            //DisplayNames
            foreach (var kv in input.DisplayNames)
            {
                application.DisplayNames.Add(new CultureInfo(kv.Key), kv.Value);
            }

            foreach (var requirement in input.Requirements)
            {
                application.Requirements.Add(requirement);
            }


            //RedirectUris
            foreach (var redirectUri in input.RedirectUris)
            {
                if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
                {
                    throw new BusinessException(L["InvalidRedirectUri", redirectUri]);
                }
                application.RedirectUris.Add(uri);
            }
            var fRedirectUri = input.RedirectUris?.FirstOrDefault();
            var fPostLogoutRedirectUri = input.PostLogoutRedirectUris?.FirstOrDefault();



            if (new[] { OpenIddictConstants.GrantTypes.AuthorizationCode, OpenIddictConstants.GrantTypes.Implicit }.All(input.GrantTypes.Contains))
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken);

                if (string.Equals(input.Type, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken);
                    application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeToken);
                }
            }

            if (!fRedirectUri.IsNullOrWhiteSpace() || !fPostLogoutRedirectUri.IsNullOrWhiteSpace())
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Logout);
            }

            foreach (var grantType in input.GrantTypes)
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

                if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode ||
                    grantType == OpenIddictConstants.GrantTypes.ClientCredentials ||
                    grantType == OpenIddictConstants.GrantTypes.Password ||
                    grantType == OpenIddictConstants.GrantTypes.RefreshToken ||
                    grantType == OpenIddictConstants.GrantTypes.DeviceCode)
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                    application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Revocation);
                    application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Introspection);
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
                    application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Device);
                }

                if (grantType == OpenIddictConstants.GrantTypes.Implicit)
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdToken);
                    if (string.Equals(input.Type, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
                    {
                        application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken);
                        application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Token);
                    }
                }
            }

            var buildInScopes = GetAllScopes();

            foreach (var scope in input.Scopes)
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

            if (fRedirectUri != null)
            {
                if (!fRedirectUri.IsNullOrEmpty())
                {
                    if (!Uri.TryCreate(fRedirectUri, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
                    {
                        throw new BusinessException(L["InvalidRedirectUri", fRedirectUri]);
                    }

                    if (application.RedirectUris.All(x => x != uri))
                    {
                        application.RedirectUris.Add(uri);
                    }
                }
            }

            if (fPostLogoutRedirectUri != null)
            {
                if (!fPostLogoutRedirectUri.IsNullOrEmpty())
                {
                    if (!Uri.TryCreate(fPostLogoutRedirectUri, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
                    {
                        throw new BusinessException(L["InvalidPostLogoutRedirectUri", fPostLogoutRedirectUri]);
                    }

                    if (application.PostLogoutRedirectUris.All(x => x != uri))
                    {
                        application.PostLogoutRedirectUris.Add(uri);
                    }
                }
            }
            return application;
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
                Type = model.Type,
                ClientUri = model.ClientUri,
                LogoUri = model.LogoUri,
                Properties = model.Properties,
            };

            foreach (var extraProperty in model.ExtraProperties)
            {
                dto.ExtraProperties.Add(extraProperty.Key, extraProperty.Value);
            }

            var displayNames = await ApplicationManager.GetDisplayNamesAsync(model);
            dto.DisplayNames = new Dictionary<string, string>(displayNames.Select(x => new KeyValuePair<string, string>(x.Key.Name, x.Value)));

            var redirectUris = await ApplicationManager.GetRedirectUrisAsync(model);
            dto.RedirectUris = redirectUris.ToList();

            var postLogoutRedirectUris = await ApplicationManager.GetPostLogoutRedirectUrisAsync(model);
            dto.PostLogoutRedirectUris = postLogoutRedirectUris.ToList();

            var requirements = await ApplicationManager.GetRequirementsAsync(model);
            dto.Requirements = requirements.ToList();

            var permissions = await ApplicationManager.GetPermissionsAsync(model);
            var commonScopes = GetAllScopes();

            foreach (var permission in permissions)
            {
                if (permission.StartsWith(OpenIddictConstants.Permissions.Prefixes.Scope))
                {
                    if (commonScopes.Contains(permission))
                    {
                        dto.Scopes.Add(permission);
                    }
                    else
                    {
                        dto.Scopes.Add(permission.RemovePreFix($"{OpenIddictConstants.Permissions.Prefixes.Scope}."));
                    }
                }

                if (permission.StartsWith(OpenIddictConstants.Permissions.Prefixes.GrantType))
                {
                    dto.GrantTypes.Add(permission.RemovePostFix($"{OpenIddictConstants.Permissions.Prefixes.GrantType}."));
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
