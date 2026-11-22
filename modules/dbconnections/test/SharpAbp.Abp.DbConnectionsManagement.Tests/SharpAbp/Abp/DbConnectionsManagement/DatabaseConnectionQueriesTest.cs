using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace SharpAbp.Abp.DbConnectionsManagement;

public class DatabaseConnectionQueriesTest : DbConnectionsManagementTestBase
{
    [Theory]
    [InlineData("unknown")]
    [InlineData("999")]
    [InlineData("MySql, PostgreSql")]
    public async Task Create_And_Update_Reject_Invalid_Provider_Names(string provider)
    {
        var service = GetRequiredService<IDatabaseConnectionInfoAppService>();
        await Assert.ThrowsAsync<UserFriendlyException>(() => service.CreateAsync(new CreateDatabaseConnectionInfoDto
        {
            Name = "invalid", DatabaseProvider = provider, ConnectionString = "example-value"
        }));
        var item = await service.CreateAsync(new CreateDatabaseConnectionInfoDto
        {
            Name = "valid", DatabaseProvider = "mysql", ConnectionString = "example-value"
        });
        Assert.Equal("MySql", (await service.GetAsync(item.Id)).DatabaseProvider);
        await Assert.ThrowsAsync<UserFriendlyException>(() => service.UpdateAsync(item.Id, new UpdateDatabaseConnectionInfoDto
        {
            Name = "valid", DatabaseProvider = provider, ConnectionString = "example-value"
        }));
    }

    [Fact]
    public async Task Defined_Numeric_Providers_Remain_Writable_And_Are_Normalized()
    {
        var service = GetRequiredService<IDatabaseConnectionInfoAppService>();
        var created = await service.CreateAsync(new CreateDatabaseConnectionInfoDto
        {
            Name = "legacy-numeric", DatabaseProvider = "0", ConnectionString = "example-value"
        });
        Assert.Equal("SqlServer", (await service.GetAsync(created.Id)).DatabaseProvider);

        await service.UpdateAsync(created.Id, new UpdateDatabaseConnectionInfoDto
        {
            Name = "legacy-numeric", DatabaseProvider = "1", ConnectionString = "updated-value"
        });
        var updated = await service.GetAsync(created.Id);
        Assert.Equal("MySql", updated.DatabaseProvider);
        Assert.Equal("updated-value", updated.ConnectionString);
    }

    [Fact]
    public async Task Queries_Filter_By_Name_And_Provider_And_Delete_Removes_Entry()
    {
        var service = new DatabaseConnectionInfoController(GetRequiredService<IDatabaseConnectionInfoAppService>());
        var created = await service.CreateAsync(new CreateDatabaseConnectionInfoDto
        {
            Name = "reporting", DatabaseProvider = "MySql", ConnectionString = "example-value"
        });
        await service.CreateAsync(new CreateDatabaseConnectionInfoDto
        {
            Name = "archive", DatabaseProvider = "PostgreSql", ConnectionString = "other-value"
        });

        var list = await service.GetListAsync(name: "reporting", databaseProvider: "MySql");
        Assert.Equal(created.Id, Assert.Single(list).Id);
        var page = await service.GetPagedListAsync(new DatabaseConnectionInfoPagedRequestDto
        {
            Name = "reporting", DatabaseProvider = "MySql", MaxResultCount = 1
        });
        Assert.Equal(1, page.TotalCount);
        Assert.Equal(created.Id, Assert.Single(page.Items).Id);
        await service.DeleteAsync(created.Id);
        Assert.Empty(await service.GetListAsync(name: "reporting"));
        await Assert.ThrowsAnyAsync<EntityNotFoundException>(() => service.GetAsync(created.Id));
    }

    [Fact]
    public async Task Duplicate_Name_Is_Rejected_Without_Creating_Another_Connection()
    {
        var service = new DatabaseConnectionInfoController(GetRequiredService<IDatabaseConnectionInfoAppService>());
        var input = new CreateDatabaseConnectionInfoDto
        {
            Name = "unique", DatabaseProvider = "MySql", ConnectionString = "example-value"
        };
        await service.CreateAsync(input);

        await Assert.ThrowsAsync<UserFriendlyException>(() => service.CreateAsync(input));

        Assert.Single(await service.GetListAsync(name: "unique"));
    }
}
