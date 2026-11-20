using System.Threading.Tasks;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.TenantGroupManagement;

public class TenantGroupQueriesTest : TenantGroupManagementTestBase
{
    [Fact]
    public async Task Group_Queries_And_Updates_Preserve_Name_And_Active_State()
    {
        var service = new TenantGroupController(GetRequiredService<ITenantGroupAppService>());
        var group = await service.CreateAsync(new CreateTenantGroupDto { Name = "query-group", IsActive = true });

        Assert.Equal(group.Id, (await service.FindByNameAsync("QUERY-GROUP")).Id);
        Assert.Equal(group.Id, Assert.Single(await service.GetListAsync(name: "query-group")).Id);
        var page = await service.GetPagedListAsync(new TenantGroupPagedRequestDto { Name = "query-group" });
        Assert.Equal(1, page.TotalCount);
        Assert.Equal(group.Id, Assert.Single(page.Items).Id);
        await Assert.ThrowsAsync<UserFriendlyException>(() => service.CreateAsync(new CreateTenantGroupDto
        {
            Name = "query-group", IsActive = true
        }));

        var updated = await service.UpdateAsync(group.Id, new UpdateTenantGroupDto { Name = "renamed-group", IsActive = false });
        Assert.Equal("renamed-group", updated.Name);
        Assert.False(updated.IsActive);
        await service.UpdateDefaultConnectionStringAsync(group.Id, "example-value");
        await service.UpdateDefaultConnectionStringAsync(group.Id, "example-value");
        await service.DeleteDefaultConnectionStringAsync(group.Id);
        Assert.Null(await service.GetDefaultConnectionStringAsync(group.Id));
    }
}
