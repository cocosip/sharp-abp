using System;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace SharpAbp.Abp.DbConnectionsManagement;

public class DatabaseConnectionMapperlyTest : DbConnectionsManagementTestBase
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Connection_Maps_Provider_Value_And_Only_Defined_Extra_Properties(bool existing)
    {
        const string property = "ConnectionMapperPublic";
        ObjectExtensionManager.Instance.AddOrUpdateProperty<DatabaseConnectionInfo, string>(property);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<DatabaseConnectionInfoDto, string>(property);
        var source = new DatabaseConnectionInfo(Guid.NewGuid(), "connection", "MySql", "example-value");
        source.SetProperty(property, "public");
        source.SetProperty("ConnectionMapperPrivate", "private");
        var mapper = GetRequiredService<IObjectMapper<DbConnectionsManagementApplicationModule>>();

        var result = existing ? mapper.Map(source, new DatabaseConnectionInfoDto()) : mapper.Map<DatabaseConnectionInfo, DatabaseConnectionInfoDto>(source);

        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Name, result.Name);
        Assert.Equal(source.DatabaseProvider, result.DatabaseProvider);
        Assert.Equal(source.ConnectionString, result.ConnectionString);
        Assert.Equal(source.CreationTime, result.CreationTime);
        Assert.Equal("public", result.ExtraProperties[property]);
        Assert.False(result.ExtraProperties.ContainsKey("ConnectionMapperPrivate"));
    }
}
