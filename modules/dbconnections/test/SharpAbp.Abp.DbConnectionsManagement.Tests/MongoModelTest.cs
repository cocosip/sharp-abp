using System;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.DbConnectionsManagement.MongoDB;
using Volo.Abp;
using Volo.Abp.MongoDB;
using Xunit;

namespace SharpAbp.Abp.DbConnectionsManagement;

public class MongoModelTest
{
    [Fact]
    public void Module_Registers_The_Mongo_Repository_Without_Connecting_To_A_Server()
    {
        var services = new ServiceCollection();
        using var application = services.AddApplication<DbConnectionsManagementMongoDbModule>();
        using var provider = services.BuildServiceProvider();

        Assert.IsType<MongoDbDatabaseConnectionInfoRepository>(provider.GetRequiredService<IDatabaseConnectionInfoRepository>());

    }

    [Fact]
    public void Context_Registers_Expected_Collection_And_Custom_Prefix_Is_Respected()
    {
        var defaults = new MongoModelBuilder();
        var context = new TestContext();
        context.ConfigureModel(defaults);
        var collection = Assert.Single(defaults.GetEntities(), entity => entity.EntityType == typeof(DatabaseConnectionInfo));
        Assert.EndsWith("DatabaseConnectionInfos", collection.CollectionName);
        var custom = new MongoModelBuilder();

        custom.ConfigureDbConnectionsManagement(options => options.CollectionPrefix = "Test_");

        Assert.Equal("Test_DatabaseConnectionInfos", Assert.Single(custom.GetEntities(), entity => entity.EntityType == typeof(DatabaseConnectionInfo)).CollectionName);
        Assert.Throws<ArgumentNullException>(() => DbConnectionsManagementMongoDbContextExtensions.ConfigureDbConnectionsManagement(null));
    }

    private sealed class TestContext : DbConnectionsManagementMongoDbContext
    {
        public void ConfigureModel(IMongoModelBuilder builder) => CreateModel(builder);
    }
}