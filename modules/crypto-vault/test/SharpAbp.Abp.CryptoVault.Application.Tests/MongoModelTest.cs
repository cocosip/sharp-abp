using System;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.CryptoVault.MongoDB;
using Volo.Abp;
using Volo.Abp.MongoDB;
using Xunit;

namespace SharpAbp.Abp.CryptoVault;

public class MongoModelTest
{
    [Fact]
    public void Module_Registers_The_Mongo_Repository_Without_Connecting_To_A_Server()
    {
        var services = new ServiceCollection();
        using var application = services.AddApplication<AbpCryptoVaultMongoDbModule>();
        using var provider = services.BuildServiceProvider();

        Assert.IsType<MongoDbRSACredsRepository>(provider.GetRequiredService<IRSACredsRepository>());
        Assert.IsType<MongoDbSM2CredsRepository>(provider.GetRequiredService<ISM2CredsRepository>());
    }

    [Fact]
    public void Context_Registers_Expected_Collection_And_Custom_Prefix_Is_Respected()
    {
        var defaults = new MongoModelBuilder();
        var context = new TestContext();
        context.ConfigureModel(defaults);
        var collection = Assert.Single(defaults.GetEntities(), entity => entity.EntityType == typeof(RSACreds));
        Assert.EndsWith("RSACreds", collection.CollectionName);
        var custom = new MongoModelBuilder();

        custom.ConfigureCryptoVault(options => options.CollectionPrefix = "Test_");

        Assert.Equal("Test_RSACreds", Assert.Single(custom.GetEntities(), entity => entity.EntityType == typeof(RSACreds)).CollectionName);
        Assert.Throws<ArgumentNullException>(() => CryptoVaultMongoDbContextExtensions.ConfigureCryptoVault(null));
    }

    private sealed class TestContext : AbpCryptoVaultMongoDbContext
    {
        public void ConfigureModel(IMongoModelBuilder builder) => CreateModel(builder);
    }
}