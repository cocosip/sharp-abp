using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Volo.Abp.OpenIddict.Scopes;

namespace SharpAbp.Abp.OpenIddict;

// Scope lookups follow the database transaction and never publish an in-flight model.
internal sealed class StoreBackedScopeCache(IOpenIddictScopeStore<OpenIddictScopeModel> store)
    : IOpenIddictScopeCache<OpenIddictScopeModel>
{
    public ValueTask AddAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public ValueTask RemoveAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public ValueTask<OpenIddictScopeModel> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        => store.FindByIdAsync(identifier, cancellationToken);

    public ValueTask<OpenIddictScopeModel> FindByNameAsync(string name, CancellationToken cancellationToken)
        => store.FindByNameAsync(name, cancellationToken);

    public IAsyncEnumerable<OpenIddictScopeModel> FindByNamesAsync(ImmutableArray<string> names, CancellationToken cancellationToken)
        => store.FindByNamesAsync(names, cancellationToken);

    public IAsyncEnumerable<OpenIddictScopeModel> FindByResourceAsync(string resource, CancellationToken cancellationToken)
        => store.FindByResourceAsync(resource, cancellationToken);
}
