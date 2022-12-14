using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.OpenIddict.Applications;

namespace SharpAbp.Abp.OpenIddict
{
    [Authorize(OpenIddictPermissions.Applications.Default)]
    public class OpenIddictApplicationAppService : OpenIddictAppServiceBase, IOpenIddictApplicationAppService
    {
        protected IAbpApplicationManager ApplicationManager { get; }
        public OpenIddictApplicationAppService(IAbpApplicationManager applicationManager)
        {
            ApplicationManager = applicationManager;
        }

        [Authorize(OpenIddictPermissions.Applications.Default)]
        public virtual async Task<OpenIddictApplicationDto> GetAsync(Guid id)
        {
            var application = (await ApplicationManager.FindByIdAsync(id.ToString("D")));
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

        //[Authorize(OpenIddictPermissions.Applications.Create)]
        //public virtual async Task<AbpApplicationDescriptorDto> CreateAsync(CreateOrUpdateAbpApplicationDescriptorDto input)
        //{

        //    var application = new AbpApplicationDescriptor()
        //    {
        //        ClientId = input.ClientId,
        //        Type = input.Type,
        //        ClientSecret = input.ClientSecret,
        //        ConsentType = input.ConsentType,
        //        DisplayName = input.DisplayName,
        //        ClientUri = input.ClientUri,
        //        LogoUri = input.LogoUri
        //    };

        //    var created = await AbpApplicationManager.CreateAsync(application);
        //    return await ToApplicationDescriptorAsync(created.As<OpenIddictApplicationModel>());
        //}


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
