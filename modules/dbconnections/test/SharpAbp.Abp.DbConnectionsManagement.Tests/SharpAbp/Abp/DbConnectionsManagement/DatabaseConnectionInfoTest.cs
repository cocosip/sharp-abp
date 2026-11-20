using System;
using Xunit;

namespace SharpAbp.Abp.DbConnectionsManagement;

public class DatabaseConnectionInfoTest
{
    [Fact]
    public void Update_And_Rename_Publish_Original_And_New_Connection_Information()
    {
        var entity = new DatabaseConnectionInfo(Guid.NewGuid(), "original", "MySql", "original-value");

        entity.Update("updated", "SqlServer", "updated-value");

        var updated = Assert.IsType<DatabaseConnectionUpdatedEto>(Assert.Single(entity.GetDistributedEvents()).EventData);
        Assert.Equal(entity.Id, updated.Id);
        Assert.Equal("original", updated.OldName);
        Assert.Equal("updated", updated.Name);
        Assert.Equal("SqlServer", updated.DatabaseProvider);
        Assert.Equal("updated-value", updated.ConnectionString);
        entity.ClearDistributedEvents();
        entity.ChangeName("renamed");
        var renamed = Assert.IsType<DatabaseConnectionNameChangedEto>(Assert.Single(entity.GetDistributedEvents()).EventData);
        Assert.Equal(entity.Id, renamed.Id);
        Assert.Equal("updated", renamed.OldName);
        Assert.Equal("renamed", renamed.Name);
        Assert.Equal("renamed", entity.Name);
    }

    [Theory]
    [InlineData("", "MySql", "value")]
    [InlineData("new", " ", "value")]
    [InlineData("new", "MySql", null)]
    public void Invalid_Update_Does_Not_Mutate_Or_Publish(string name, string provider, string connectionString)
    {
        var entity = new DatabaseConnectionInfo(Guid.NewGuid(), "original", "MySql", "original-value");

        Assert.ThrowsAny<ArgumentException>(() => entity.Update(name, provider, connectionString));

        Assert.Equal("original", entity.Name);
        Assert.Equal("MySql", entity.DatabaseProvider);
        Assert.Equal("original-value", entity.ConnectionString);
        Assert.Empty(entity.GetDistributedEvents());
    }
}
