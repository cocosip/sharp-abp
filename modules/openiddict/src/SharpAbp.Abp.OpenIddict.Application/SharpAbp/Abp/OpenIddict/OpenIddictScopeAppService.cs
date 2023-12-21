using Microsoft.AspNetCore.Authorization;
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
using Volo.Abp.OpenIddict.Scopes;

namespace SharpAbp.Abp.OpenIddict
{
    [Authorize(OpenIddictPermissions.Scopes.Default)]
    public class OpenIddictScopeAppService : OpenIddictAppServiceBase, IOpenIddictScopeAppService
    {
        protected IOpenIddictScopeStore<OpenIddictScopeModel> OpenIddictScopeStore { get; }
        protected IOpenIddictScopeRepository OpenIddictScopeRepository { get; }
        public OpenIddictScopeAppService(
            IOpenIddictScopeStore<OpenIddictScopeModel> openIddictScopeStore,
            IOpenIddictScopeRepository openIddictScopeRepository)
        {
            OpenIddictScopeStore = openIddictScopeStore;
            OpenIddictScopeRepository = openIddictScopeRepository;
        }

        [Authorize(OpenIddictPermissions.Scopes.Default)]
        public virtual async Task<OpenIddictScopeDto> GetAsync(Guid id)
        {
            var scope = await OpenIddictScopeRepository.GetAsync(id);
            return await ToScopeDtoAsync(scope.ToModel());
        }

        [Authorize(OpenIddictPermissions.Scopes.Default)]
        public virtual async Task<OpenIddictScopeDto> FindByNameAsync(string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            var scope = await OpenIddictScopeRepository.FindByNameAsync(name);
            return await ToScopeDtoAsync(scope.ToModel());
        }

        [Authorize(OpenIddictPermissions.Scopes.Default)]
        public virtual async Task<PagedResultDto<OpenIddictScopeDto>> GetPagedListAsync(OpenIddictScopePagedRequestDto input)
        {
            var count = await OpenIddictScopeRepository.GetCountAsync(input.Filter);
            var scopes = await OpenIddictScopeRepository.GetListAsync(input.Sorting, input.SkipCount, input.MaxResultCount, input.Filter);
            var scopeDtos = new List<OpenIddictScopeDto>();
            foreach (var scope in scopes)
            {
                scopeDtos.Add(await ToScopeDtoAsync(scope.ToModel()));
            }

            return new PagedResultDto<OpenIddictScopeDto>(count, scopeDtos);
        }

        [Authorize(OpenIddictPermissions.Scopes.Default)]
        public virtual async Task<List<OpenIddictScopeDto>> GetListAsync()
        {
            var scopes = await OpenIddictScopeRepository.GetListAsync();
            var scopeDtos = new List<OpenIddictScopeDto>();
            foreach (var scope in scopes)
            {
                scopeDtos.Add(await ToScopeDtoAsync(scope.ToModel()));
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
            };

            foreach (var extraProperty in input.ExtraProperties)
            {
                model.ExtraProperties.Add(extraProperty.Key, extraProperty.Value);
            }

            await OpenIddictScopeStore.SetPropertiesAsync(model, input.Properties.ToImmutableDictionary(), CancellationToken.None);
            await OpenIddictScopeStore.SetResourcesAsync(model, input.Resources.ToImmutableArray(), CancellationToken.None);

            var descriptions = new Dictionary<CultureInfo, string>(input.Descriptions.Select(x => new KeyValuePair<CultureInfo, string>(new CultureInfo(x.Key), x.Value)));
            await OpenIddictScopeStore.SetDescriptionsAsync(model, descriptions.ToImmutableDictionary(), CancellationToken.None);

            var displayNames = new Dictionary<CultureInfo, string>(input.DisplayNames.Select(x => new KeyValuePair<CultureInfo, string>(new CultureInfo(x.Key), x.Value)));
            await OpenIddictScopeStore.SetDisplayNamesAsync(model, displayNames.ToImmutableDictionary(), CancellationToken.None);

            await OpenIddictScopeStore.CreateAsync(model, CancellationToken.None);

            return await ToScopeDtoAsync(model);
        }

        [Authorize(OpenIddictPermissions.Scopes.Update)]
        public virtual async Task<OpenIddictScopeDto> UpdateAsync(Guid id, UpdateOpenIddictScopeDto input)
        {
            var model = await OpenIddictScopeStore.FindByIdAsync(id.ToString("D"), CancellationToken.None);
            model.ExtraProperties.Clear();
            foreach (var extraProperty in input.ExtraProperties)
            {
                model.ExtraProperties.Add(extraProperty.Key, extraProperty.Value);
            }

            await OpenIddictScopeStore.SetNameAsync(model, input.Name, CancellationToken.None);
            await OpenIddictScopeStore.SetDisplayNameAsync(model, input.DisplayName, CancellationToken.None);
            await OpenIddictScopeStore.SetDescriptionAsync(model, input.Description, CancellationToken.None);

            await OpenIddictScopeStore.SetPropertiesAsync(model, input.Properties.ToImmutableDictionary(), CancellationToken.None);
            await OpenIddictScopeStore.SetResourcesAsync(model, input.Resources.ToImmutableArray(), CancellationToken.None);

            var descriptions = new Dictionary<CultureInfo, string>(input.Descriptions.Select(x => new KeyValuePair<CultureInfo, string>(new CultureInfo(x.Key), x.Value)));
            await OpenIddictScopeStore.SetDescriptionsAsync(model, descriptions.ToImmutableDictionary(), CancellationToken.None);

            var displayNames = new Dictionary<CultureInfo, string>(input.DisplayNames.Select(x => new KeyValuePair<CultureInfo, string>(new CultureInfo(x.Key), x.Value)));
            await OpenIddictScopeStore.SetDisplayNamesAsync(model, displayNames.ToImmutableDictionary(), CancellationToken.None);

            await OpenIddictScopeStore.UpdateAsync(model, CancellationToken.None);
            return await ToScopeDtoAsync(model, CancellationToken.None);
        }

        [Authorize(OpenIddictPermissions.Scopes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await OpenIddictScopeRepository.DeleteAsync(id);
        }

        protected virtual async Task<OpenIddictScopeDto> ToScopeDtoAsync(OpenIddictScopeModel model, CancellationToken cancellationToken = default)
        {
            var dto = new OpenIddictScopeDto()
            {
                Id = model.Id,
                Description = model.Description,
                DisplayName = model.DisplayName,
                Name = model.Name
            };

            dto.Properties = new Dictionary<string, JsonElement>(await OpenIddictScopeStore.GetPropertiesAsync(model, cancellationToken));

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
