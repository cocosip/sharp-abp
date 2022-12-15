using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.OpenIddict.Scopes;

namespace SharpAbp.Abp.OpenIddict
{
    [Authorize(OpenIddictPermissions.Scopes.Default)]
    public class OpenIddictScopeAppService : OpenIddictAppServiceBase, IOpenIddictScopeAppService
    {
        protected IOpenIddictScopeStore<OpenIddictScopeModel> OpenIddictScopeStore { get; }
        public OpenIddictScopeAppService(IOpenIddictScopeStore<OpenIddictScopeModel> openIddictScopeStore)
        {
            OpenIddictScopeStore = openIddictScopeStore;
        }

        [Authorize(OpenIddictPermissions.Scopes.Default)]
        public virtual async Task<OpenIddictScopeDto> GetAsync(Guid id)
        {
            var model = await OpenIddictScopeStore.FindByIdAsync(id.ToString("D"), CancellationToken.None);
            return await ToScopeDtoAsync(model);
        }

        [Authorize(OpenIddictPermissions.Scopes.Default)]
        public virtual async Task<OpenIddictScopeDto> FindByNameAsync(string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            var model = await OpenIddictScopeStore.FindByNameAsync(name, CancellationToken.None);
            return await ToScopeDtoAsync(model);
        }

        [Authorize(OpenIddictPermissions.Scopes.Default)]
        public virtual async Task<PagedResultDto<OpenIddictScopeDto>> GetPagedListAsync(PagedAndSortedResultRequestDto input)
        {
            var count = await OpenIddictScopeStore.CountAsync(CancellationToken.None);
            var scopes = OpenIddictScopeStore.ListAsync(input.MaxResultCount, input.SkipCount, CancellationToken.None);

            var scopeDtos = new List<OpenIddictScopeDto>();
            await foreach (var model in scopes)
            {
                scopeDtos.Add(await ToScopeDtoAsync(model));
            }

            return new PagedResultDto<OpenIddictScopeDto>(count, scopeDtos);
        }

        [Authorize(OpenIddictPermissions.Scopes.Default)]
        public virtual async Task<List<OpenIddictScopeDto>> GetListAsync()
        {
            var scopes = OpenIddictScopeStore.ListAsync(int.MaxValue, 0, CancellationToken.None);
            var scopeDtos = new List<OpenIddictScopeDto>();
            await foreach (var model in scopes)
            {
                scopeDtos.Add(await ToScopeDtoAsync(model));
            }
            return scopeDtos;
        }

        [Authorize(OpenIddictPermissions.Scopes.Create)]
        public virtual async Task<OpenIddictScopeDto> CreateAsync(CreateOpenIddictScopeDto input)
        {
            var model = new OpenIddictScopeModel()
            {
                Id = GuidGenerator.Create(),
                Name = input.Name,
                DisplayName = input.DisplayName,
                Description = input.Description,
                Properties = input.Properties,
            };

            foreach (var extraProperty in input.ExtraProperties)
            {
                model.ExtraProperties.Add(extraProperty.Key, extraProperty.Value);
            }

            await OpenIddictScopeStore.SetResourcesAsync(model, input.Resources.ToImmutableArray(), CancellationToken.None);

            var descriptions = new Dictionary<CultureInfo, string>();
            foreach (var description in input.Descriptions)
            {
                descriptions.Add(new CultureInfo(description.Key, true), description.Value);
            }
            await OpenIddictScopeStore.SetDescriptionsAsync(model, descriptions.ToImmutableDictionary(), CancellationToken.None);

            var displayNames = new Dictionary<CultureInfo, string>();
            foreach (var displayName in input.DisplayNames)
            {
                displayNames.Add(new CultureInfo(displayName.Key, true), displayName.Value);
            }
            await OpenIddictScopeStore.SetDisplayNamesAsync(model, displayNames.ToImmutableDictionary(), CancellationToken.None);

            await OpenIddictScopeStore.CreateAsync(model, CancellationToken.None);

            return await ToScopeDtoAsync(model);
        }

        [Authorize(OpenIddictPermissions.Scopes.Update)]
        public virtual async Task<OpenIddictScopeDto> UpdateAsync(Guid id, UpdateOpenIddictScopeDto input)
        {
            var model = await OpenIddictScopeStore.FindByIdAsync(id.ToString("D"), CancellationToken.None);
            await OpenIddictScopeStore.SetNameAsync(model, input.Name, CancellationToken.None);
            await OpenIddictScopeStore.SetDisplayNameAsync(model, input.DisplayName, CancellationToken.None);
            await OpenIddictScopeStore.SetDescriptionAsync(model, input.Description, CancellationToken.None);
            await OpenIddictScopeStore.SetResourcesAsync(model, input.Resources.ToImmutableArray(), CancellationToken.None);

            var descriptions = new Dictionary<CultureInfo, string>();
            foreach (var description in input.Descriptions)
            {
                descriptions.Add(new CultureInfo(description.Key, true), description.Value);
            }
            await OpenIddictScopeStore.SetDescriptionsAsync(model, descriptions.ToImmutableDictionary(), CancellationToken.None);

            var displayNames = new Dictionary<CultureInfo, string>();
            foreach (var displayName in input.DisplayNames)
            {
                displayNames.Add(new CultureInfo(displayName.Key, true), displayName.Value);
            }
            await OpenIddictScopeStore.SetDisplayNamesAsync(model, displayNames.ToImmutableDictionary(), CancellationToken.None);

            await OpenIddictScopeStore.UpdateAsync(model, CancellationToken.None);

            return await ToScopeDtoAsync(model, CancellationToken.None);
        }

        [Authorize(OpenIddictPermissions.Scopes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            var model = await OpenIddictScopeStore.FindByIdAsync(id.ToString("D"), CancellationToken.None);
            if (model == null)
            {
                throw new EntityNotFoundException(typeof(OpenIddictScope), id);
            }
            await OpenIddictScopeStore.DeleteAsync(model, CancellationToken.None);
        }

        protected virtual async Task<OpenIddictScopeDto> ToScopeDtoAsync(OpenIddictScopeModel model, CancellationToken cancellationToken = default)
        {
            var dto = new OpenIddictScopeDto()
            {
                Id = model.Id,
                Description = model.Description,
                DisplayName = model.DisplayName,
                Name = model.Name,
                Properties = model.Properties
            };

            var displayNames = await OpenIddictScopeStore.GetDisplayNamesAsync(model, cancellationToken);
            dto.DisplayNames = new Dictionary<string, string>(displayNames.Select(x => new KeyValuePair<string, string>(x.Key.Name, x.Value)));

            var descriptions = await OpenIddictScopeStore.GetDescriptionsAsync(model, cancellationToken);
            dto.Descriptions = new Dictionary<string, string>(descriptions.Select(x => new KeyValuePair<string, string>(x.Key.Name, x.Value)));

            var resources = await OpenIddictScopeStore.GetResourcesAsync(model, cancellationToken);
            dto.Resources = resources.ToList();

            return dto;
        }

    }
}
