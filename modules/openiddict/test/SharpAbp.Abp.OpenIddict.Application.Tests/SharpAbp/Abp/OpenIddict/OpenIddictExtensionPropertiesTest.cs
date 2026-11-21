using System.Text.Json;
using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Scopes;
using Xunit;

namespace SharpAbp.Abp.OpenIddict;

public class OpenIddictExtensionPropertiesTest : OpenIddictApplicationTestBase
{
    private const string Defined = "ExtensionRoundTrip";
    private const string Reserved = "ServerManagedValue";

    [Fact]
    public async Task Application_Accepts_Null_Properties_And_Preserves_Stored_Values()
    {
        var service = GetRequiredService<IOpenIddictApplicationAppService>();
        var input = ApplicationInput();
        input.Properties = null;
        var created = await service.CreateAsync(input);
        Assert.Empty(created.Properties);

        input.Properties = new() { ["existingJson"] = JsonSerializer.SerializeToElement("preserve") };
        await service.UpdateAsync(created.Id, input);
        input.Properties = null;
        var updated = await service.UpdateAsync(created.Id, input);

        Assert.Equal("preserve", updated.Properties["existingJson"].GetString());
    }

    [Fact]
    public async Task Application_Properties_Merge_And_Defined_Extensions_Round_Trip()
    {
        ObjectExtensionManager.Instance.AddOrUpdateProperty<OpenIddictApplication, string>(Defined);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<OpenIddictApplicationDto, string>(Defined);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<CreateOrUpdateOpenIddictApplicationDto, string>(Defined);
        var service = GetRequiredService<IOpenIddictApplicationAppService>();
        var input = ApplicationInput();
        input.Properties["existingJson"] = JsonSerializer.SerializeToElement("preserve");
        input.ExtraProperties[Defined] = "created";
        input.ExtraProperties[Reserved] = "discard";

        var created = await service.CreateAsync(input);

        Assert.Equal("created", created.ExtraProperties[Defined]);
        Assert.False(created.ExtraProperties.ContainsKey(Reserved));
        Assert.Equal("preserve", created.Properties["existingJson"].GetString());
        var update = ApplicationInput();
        update.Properties["newJson"] = JsonSerializer.SerializeToElement(42);
        update.ExtraProperties[Defined] = "updated";
        await service.UpdateAsync(created.Id, update);

        var loaded = await service.GetAsync(created.Id);
        Assert.Equal("updated", loaded.ExtraProperties[Defined]);
        Assert.Equal("preserve", loaded.Properties["existingJson"].GetString());
        Assert.Equal(42, loaded.Properties["newJson"].GetInt32());
    }

    [Fact]
    public async Task Scope_Only_Returns_Defined_Extensions_And_Preserves_Existing_Json_Properties()
    {
        ObjectExtensionManager.Instance.AddOrUpdateProperty<OpenIddictScope, string>(Defined);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<OpenIddictScopeDto, string>(Defined);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<CreateOpenIddictScopeDto, string>(Defined);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<UpdateOpenIddictScopeDto, string>(Defined);
        var service = GetRequiredService<IOpenIddictScopeAppService>();
        var input = new CreateOpenIddictScopeDto { Name = "extension-scope" };
        input.Properties["existingJson"] = JsonSerializer.SerializeToElement("preserve");
        input.ExtraProperties[Defined] = "created";
        input.ExtraProperties[Reserved] = "discard";

        var created = await service.CreateAsync(input);

        Assert.Equal("created", created.ExtraProperties[Defined]);
        Assert.False(created.ExtraProperties.ContainsKey(Reserved));
        var update = new UpdateOpenIddictScopeDto { Name = input.Name };
        update.Properties["newJson"] = JsonSerializer.SerializeToElement(true);
        update.ExtraProperties[Defined] = "updated";
        await service.UpdateAsync(created.Id, update);

        var loaded = await service.GetAsync(created.Id);
        Assert.Equal("updated", loaded.ExtraProperties[Defined]);
        Assert.Equal("preserve", loaded.Properties["existingJson"].GetString());
        Assert.True(loaded.Properties["newJson"].GetBoolean());
    }

    private static CreateOrUpdateOpenIddictApplicationDto ApplicationInput()
    {
        return new CreateOrUpdateOpenIddictApplicationDto
        {
            ClientId = "extension-client",
            ClientType = OpenIddictConstants.ClientTypes.Public,
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit
        };
    }
}
