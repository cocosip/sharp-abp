using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Volo.Abp.Caching;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.Abp.OpenIddict;

public class OpenIddictScopeLifecycleTest : OpenIddictApplicationTestBase
{
    [Fact]
    public async Task Committed_Scope_Changes_Are_Visible_To_The_Runtime_Manager()
    {
        var service = GetRequiredService<IOpenIddictScopeAppService>();
        var manager = GetRequiredService<IOpenIddictScopeManager>();
        var scope = await service.CreateAsync(new CreateOpenIddictScopeDto { Name = "old-name", Resources = ["old-resource"] });
        Assert.NotNull(await manager.FindByNameAsync(scope.Name));
        var legacyCache = GetRequiredService<AbpOpenIddictScopeCache>();
        var legacyEntries = GetRequiredService<IDistributedCache<OpenIddictScopeModel>>();
        Assert.NotNull(await legacyCache.FindByNameAsync(scope.Name, default));

        await service.UpdateAsync(scope.Id, new UpdateOpenIddictScopeDto { Name = "new-name", Resources = ["new-resource"] });

        Assert.Null(await legacyEntries.GetAsync("FindByNameAsync_old-name"));
        Assert.Null(await manager.FindByNameAsync(scope.Name));
        var updated = await manager.FindByNameAsync("new-name");
        Assert.NotNull(updated);
        Assert.Equal(new[] { "new-resource" }, await manager.GetResourcesAsync(updated));

        await service.DeleteAsync(scope.Id);

        Assert.Null(await manager.FindByNameAsync("new-name"));
        Assert.Null(await manager.FindByIdAsync(scope.Id.ToString("D")));
    }

    [Fact]
    public async Task Rolled_Back_Scope_Changes_Leave_Only_Committed_Values_Visible()
    {
        var service = GetRequiredService<IOpenIddictScopeAppService>();
        var manager = GetRequiredService<IOpenIddictScopeManager>();
        var scope = await service.CreateAsync(new CreateOpenIddictScopeDto { Name = "committed", Resources = ["committed-resource"] });
        var legacyCache = GetRequiredService<AbpOpenIddictScopeCache>();
        var legacyEntries = GetRequiredService<IDistributedCache<OpenIddictScopeModel>>();
        Assert.NotNull(await legacyCache.FindByNameAsync(scope.Name, default));

        using (var unit = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: true))
        {
            await service.UpdateAsync(scope.Id, new UpdateOpenIddictScopeDto { Name = "rolled-back" });
            Assert.NotNull(await manager.FindByNameAsync("rolled-back"));
            await service.CreateAsync(new CreateOpenIddictScopeDto { Name = "rolled-back-create" });
            Assert.NotNull(await manager.FindByNameAsync("rolled-back-create"));
            await unit.RollbackAsync();
        }

        Assert.Null(await manager.FindByNameAsync("rolled-back"));
        Assert.Null(await manager.FindByNameAsync("rolled-back-create"));
        Assert.NotNull(await legacyEntries.GetAsync("FindByNameAsync_committed"));
        var original = await manager.FindByNameAsync(scope.Name);
        Assert.NotNull(original);
        Assert.Equal(new[] { "committed-resource" }, await manager.GetResourcesAsync(original));
    }
}
