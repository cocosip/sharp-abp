using System;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.TenantGroupManagement.MongoDB;
using Volo.Abp;
using Volo.Abp.MongoDB;
using Xunit;

namespace SharpAbp.Abp.TenantGroupManagement;

public class MongoModelTest
{
    [Fact]
    public void Module_Registers_The_Mongo_Repository_Without_Connecting_To_A_Server()
    {
        var services = new ServiceCollection();
        using var application = services.AddApplication<TenantGroupManagementMongoDbModule>();
        using var provider = services.BuildServiceProvider();

        Assert.IsType<MongoDbTenantGroupRepository>(provider.GetRequiredService<ITenantGroupRepository>());

    }

    [Fact]
    public void Context_Registers_Expected_Collection_And_Custom_Prefix_Is_Respected()
    {
        var defaults = new MongoModelBuilder();
        var context = new TestContext();
        context.ConfigureModel(defaults);
        var collection = Assert.Single(defaults.GetEntities(), entity => entity.EntityType == typeof(TenantGroup));
        Assert.EndsWith("TenantGroups", collection.CollectionName);
        var custom = new MongoModelBuilder();

        custom.ConfigureTenantGroupManagement(options => options.CollectionPrefix = "Test_");

        Assert.Equal("Test_TenantGroups", Assert.Single(custom.GetEntities(), entity => entity.EntityType == typeof(TenantGroup)).CollectionName);
        Assert.Throws<ArgumentNullException>(() => TenantGroupManagementMongoDbContextExtensions.ConfigureTenantGroupManagement(null));
    }

    private sealed class TestContext : TenantGroupManagementMongoDbContext
    {
        public void ConfigureModel(IMongoModelBuilder builder) => CreateModel(builder);
    }
}