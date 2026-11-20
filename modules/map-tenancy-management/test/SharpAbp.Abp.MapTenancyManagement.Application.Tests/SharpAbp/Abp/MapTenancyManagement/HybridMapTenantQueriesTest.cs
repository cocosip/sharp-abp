using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace SharpAbp.Abp.MapTenancyManagement;

public class HybridMapTenantQueriesTest : MapTenancyManagementApplicationTestBase
{
    [Fact]
    public async Task Queries_Update_And_Connection_String_Operations_Preserve_Tenant_Mapping()
    {
        var service = new HybridMapTenantController(GetRequiredService<IHybridMapTenantAppService>());
        var created = await service.CreateAsync(new CreateHybridMapTenantDto
        {
            Name = "hybrid-query", AdminEmailAddress = "hybrid@example.com", AdminPassword = "1q2w3E*",
            Code = "query-code", MapCode = "query-map"
        });

        Assert.Equal(created.Id, Assert.Single(await service.GetAllAsync()).Id);
        var page = await service.GetListAsync(new MapTenantPagedRequestDto { Code = "query-code" });
        Assert.Equal(1, page.TotalCount);
        Assert.Equal(created.Id, Assert.Single(page.Items).Id);
        var search = await service.SearchAsync(new MapTenantPagedRequestDto { MapCode = "query-map" });
        Assert.Equal(created.Id, Assert.Single(search.Items).Id);

        var updated = await service.UpdateAsync(created.Id, new UpdateHybridMapTenantDto
        {
            Name = "renamed-hybrid", Code = "renamed-code", MapCode = "renamed-map"
        });
        Assert.Equal(created.TenantId, updated.TenantId);
        Assert.Equal("renamed-hybrid", updated.TenantName);
        Assert.Equal("renamed-code", updated.Code);
        Assert.Equal("renamed-map", updated.MapCode);
        Assert.Null(await service.GetDefaultConnectionStringAsync(created.Id));
        await service.UpdateDefaultConnectionStringAsync(created.Id, "example-value");
        Assert.Equal("example-value", await service.GetDefaultConnectionStringAsync(created.Id));
        await service.DeleteDefaultConnectionStringAsync(created.Id);
        Assert.Null(await service.GetDefaultConnectionStringAsync(created.Id));

        using (GetRequiredService<ICurrentTenant>().Change(created.TenantId))
        {
            Assert.Equal(created.Id, (await service.CurrentAsync()).Id);
        }
        Assert.Equal(System.Guid.Empty, (await service.CurrentAsync()).Id);
    }
}
