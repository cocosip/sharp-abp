using System;
using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Tokens;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.Abp.OpenIddict;

public class OpenIddictApplicationLifecycleTest : OpenIddictApplicationTestBase
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Blank_Confidential_Client_Secret_Preserves_Authentication(string secret)
    {
        var service = GetRequiredService<IOpenIddictApplicationAppService>();
        var manager = GetRequiredService<IAbpApplicationManager>();
        var created = await service.CreateAsync(CreateInput(OpenIddictConstants.ClientTypes.Confidential, "originalsecret"));

        await service.UpdateAsync(created.Id, CreateInput(OpenIddictConstants.ClientTypes.Confidential, secret));

        var updated = await manager.FindByIdAsync(created.Id.ToString("D"));
        Assert.True(await manager.ValidateClientSecretAsync(updated, "originalsecret"));
    }

    [Fact]
    public async Task Converting_Confidential_Client_To_Public_Clears_Secret()
    {
        var service = GetRequiredService<IOpenIddictApplicationAppService>();
        var manager = GetRequiredService<IAbpApplicationManager>();
        var created = await service.CreateAsync(CreateInput(OpenIddictConstants.ClientTypes.Confidential, "originalsecret"));

        await service.UpdateAsync(created.Id, CreateInput(OpenIddictConstants.ClientTypes.Public, null));

        var updated = (OpenIddictApplicationModel)await manager.FindByIdAsync(created.Id.ToString("D"));
        Assert.Equal(OpenIddictConstants.ClientTypes.Public, updated.ClientType);
        Assert.Null(updated.ClientSecret);
        Assert.False(await manager.ValidateClientSecretAsync(updated, "originalsecret"));
    }

    [Fact]
    public async Task Deleting_Client_Removes_The_Application_From_Manager_Cache()
    {
        var service = GetRequiredService<IOpenIddictApplicationAppService>();
        var manager = GetRequiredService<IAbpApplicationManager>();
        var created = await service.CreateAsync(CreateInput(OpenIddictConstants.ClientTypes.Confidential, "originalsecret"));
        Assert.NotNull(await manager.FindByClientIdAsync(created.ClientId));

        await service.DeleteAsync(created.Id);

        Assert.Null(await manager.FindByIdAsync(created.Id.ToString("D")));
        Assert.Null(await manager.FindByClientIdAsync(created.ClientId));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Client_And_Token_Deletion_Follow_The_Callers_Transaction(bool commit)
    {
        var service = GetRequiredService<IOpenIddictApplicationAppService>();
        var repository = GetRequiredService<IOpenIddictApplicationRepository>();
        var tokens = GetRequiredService<IOpenIddictTokenRepository>();
        var manager = GetRequiredService<IAbpApplicationManager>();
        var cache = GetRequiredService<IOpenIddictApplicationCache<OpenIddictApplicationModel>>();
        var units = GetRequiredService<IUnitOfWorkManager>();
        var created = await service.CreateAsync(CreateInput(OpenIddictConstants.ClientTypes.Confidential, "originalsecret"));
        var original = (OpenIddictApplicationModel)await manager.FindByClientIdAsync(created.ClientId);
        var tokenId = Guid.NewGuid();
        using (var seed = units.Begin(requiresNew: true, isTransactional: false))
        {
            await tokens.InsertAsync(new OpenIddictToken(tokenId) { ApplicationId = created.Id }, autoSave: true);
            await seed.CompleteAsync();
        }

        using (var unit = units.Begin(requiresNew: true, isTransactional: true))
        {
            await service.DeleteAsync(created.Id);
            Assert.Null(await repository.FindAsync(created.Id));
            Assert.Null(await tokens.FindAsync(tokenId));
            // Another host can refill the cache from its last committed view before this transaction commits.
            await cache.AddAsync(original, default);
            if (commit)
            {
                await unit.CompleteAsync();
            }
            else
            {
                await unit.RollbackAsync();
            }
        }

        using var verification = units.Begin(requiresNew: true, isTransactional: false);
        Assert.Equal(!commit, await repository.FindAsync(created.Id) != null);
        Assert.Equal(!commit, await tokens.FindAsync(tokenId) != null);
        Assert.Equal(!commit, await manager.FindByClientIdAsync(created.ClientId) != null);
    }

    private static CreateOrUpdateOpenIddictApplicationDto CreateInput(string clientType, string secret) => new()
    {
        ClientId = "lifecycleclient",
        ClientType = clientType,
        ClientSecret = secret,
        ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
        DisplayName = "Lifecycle client"
    };
}
