using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace SharpAbp.Abp.OpenIddict;

public class OpenIddictScopeQueryTest : OpenIddictApplicationTestBase
{
    [Fact]
    public async Task Paging_Filters_Scopes_And_Retains_Resources()
    {
        var service = new OpenIddictScopeController(GetRequiredService<IOpenIddictScopeAppService>());
        var created = await service.CreateAsync(new CreateOpenIddictScopeDto
        {
            Name = "orders", DisplayName = "Orders", Resources = ["orders-api"]
        });
        await service.CreateAsync(new CreateOpenIddictScopeDto { Name = "customers", DisplayName = "Customers" });

        var page = await service.GetPagedListAsync(new OpenIddictScopePagedRequestDto { Filter = "orders" });

        Assert.Equal(1, page.TotalCount);
        var scope = Assert.Single(page.Items);
        Assert.Equal(created.Id, scope.Id);
        Assert.Equal("Orders", scope.DisplayName);
        Assert.Equal("orders-api", Assert.Single(scope.Resources));
        Assert.Equal(2, (await service.GetListAsync()).Count);
    }

    [Fact]
    public async Task Missing_Scope_Cannot_Be_Found_Or_Updated()
    {
        var service = new OpenIddictScopeController(GetRequiredService<IOpenIddictScopeAppService>());

        await Assert.ThrowsAnyAsync<EntityNotFoundException>(() => service.FindByNameAsync("missing"));
        await Assert.ThrowsAnyAsync<EntityNotFoundException>(() => service.UpdateAsync(Guid.NewGuid(),
            new UpdateOpenIddictScopeDto { Name = "missing" }));
    }
}
