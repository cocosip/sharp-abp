using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Volo.Abp.OpenIddict.Applications;
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

    private static CreateOrUpdateOpenIddictApplicationDto CreateInput(string clientType, string secret) => new()
    {
        ClientId = "lifecycleclient",
        ClientType = clientType,
        ClientSecret = secret,
        ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
        DisplayName = "Lifecycle client"
    };
}
