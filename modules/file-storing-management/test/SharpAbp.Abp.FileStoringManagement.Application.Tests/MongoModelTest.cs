using System;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.FileStoringManagement.MongoDB;
using Volo.Abp;
using Volo.Abp.MongoDB;
using Xunit;

namespace SharpAbp.Abp.FileStoringManagement;

public class MongoModelTest
{
    [Fact]
    public void Module_Registers_The_Mongo_Repository_Without_Connecting_To_A_Server()
    {
        var services = new ServiceCollection();
        using var application = services.AddApplication<FileStoringManagementMongoDbModule>();
        using var provider = services.BuildServiceProvider();

        Assert.IsType<MongoDbFileStoringContainerRepository>(provider.GetRequiredService<IFileStoringContainerRepository>());

    }

    [Fact]
    public void Context_Registers_Expected_Collection_And_Custom_Prefix_Is_Respected()
    {
        var defaults = new MongoModelBuilder();
        var context = new TestContext();
        context.ConfigureModel(defaults);
        var collection = Assert.Single(defaults.GetEntities(), entity => entity.EntityType == typeof(FileStoringContainer));
        Assert.EndsWith("FileStoringContainers", collection.CollectionName);
        var custom = new MongoModelBuilder();

        custom.ConfigureFileStoringManagement(options => options.CollectionPrefix = "Test_");

        Assert.Equal("Test_FileStoringContainers", Assert.Single(custom.GetEntities(), entity => entity.EntityType == typeof(FileStoringContainer)).CollectionName);
        Assert.Throws<ArgumentNullException>(() => FileStoringManagementMongoDbContextExtensions.ConfigureFileStoringManagement(null));
    }

    private sealed class TestContext : FileStoringManagementMongoDbContext
    {
        public void ConfigureModel(IMongoModelBuilder builder) => CreateModel(builder);
    }
}