using System;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.TransformSecurityManagement.MongoDB;
using Volo.Abp;
using Volo.Abp.MongoDB;
using Xunit;

namespace SharpAbp.Abp.TransformSecurityManagement;

public class MongoModelTest
{
    [Fact]
    public void Module_Registers_The_Mongo_Repository_Without_Connecting_To_A_Server()
    {
        var services = new ServiceCollection();
        using var application = services.AddApplication<AbpTransformSecurityManagementMongoDbModule>();
        using var provider = services.BuildServiceProvider();

        Assert.IsType<MongoDbSecurityCredentialInfoRepository>(provider.GetRequiredService<ISecurityCredentialInfoRepository>());

    }

    [Fact]
    public void Context_Registers_Expected_Collection_And_Custom_Prefix_Is_Respected()
    {
        var defaults = new MongoModelBuilder();
        var context = new TestContext();
        context.ConfigureModel(defaults);
        var collection = Assert.Single(defaults.GetEntities(), entity => entity.EntityType == typeof(SecurityCredentialInfo));
        Assert.EndsWith("SecurityCredentialInfos", collection.CollectionName);
        var custom = new MongoModelBuilder();

        custom.ConfigureTransformSecurityManagement(options => options.CollectionPrefix = "Test_");

        Assert.Equal("Test_SecurityCredentialInfos", Assert.Single(custom.GetEntities(), entity => entity.EntityType == typeof(SecurityCredentialInfo)).CollectionName);
        Assert.Throws<ArgumentNullException>(() => TransformSecurityManagementMongoDbContextExtensions.ConfigureTransformSecurityManagement(null));
    }

    private sealed class TestContext : AbpTransformSecurityManagementMongoDbContext
    {
        public void ConfigureModel(IMongoModelBuilder builder) => CreateModel(builder);
    }
}