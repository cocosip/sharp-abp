using System;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MapTenancyManagement.MongoDB;
using Volo.Abp;
using Volo.Abp.MongoDB;
using Xunit;

namespace SharpAbp.Abp.MapTenancyManagement;

public class MongoModelTest
{
    [Fact]
    public void Module_Registers_The_Mongo_Repository_Without_Connecting_To_A_Server()
    {
        var services = new ServiceCollection();
        using var application = services.AddApplication<MapTenancyManagementMongoDbModule>();
        using var provider = services.BuildServiceProvider();

        Assert.IsType<MongoDbMapTenantRepository>(provider.GetRequiredService<IMapTenantRepository>());

    }

    [Fact]
    public void Context_Registers_Expected_Collection_And_Custom_Prefix_Is_Respected()
    {
        var defaults = new MongoModelBuilder();
        var context = new TestContext();
        context.ConfigureModel(defaults);
        var collection = Assert.Single(defaults.GetEntities(), entity => entity.EntityType == typeof(MapTenant));
        Assert.EndsWith("MapTenants", collection.CollectionName);
        var custom = new MongoModelBuilder();

        custom.ConfigureMapTenancyManagement(options => options.CollectionPrefix = "Test_");

        Assert.Equal("Test_MapTenants", Assert.Single(custom.GetEntities(), entity => entity.EntityType == typeof(MapTenant)).CollectionName);
        Assert.Throws<ArgumentNullException>(() => MapTenancyManagementMongoDbContextExtensions.ConfigureMapTenancyManagement(null));
    }

    private sealed class TestContext : MapTenancyManagementMongoDbContext
    {
        public void ConfigureModel(IMongoModelBuilder builder) => CreateModel(builder);
    }
}