using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenIddict.Abstractions;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;
using Volo.Abp.PermissionManagement;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.Uow;
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
    public async Task Application_Properties_And_Defined_Extensions_Round_Trip()
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
        update.Properties["existingJson"] = JsonSerializer.SerializeToElement("preserve");
        update.Properties["newJson"] = JsonSerializer.SerializeToElement(42);
        update.ExtraProperties[Defined] = "updated";
        await service.UpdateAsync(created.Id, update);

        var loaded = await service.GetAsync(created.Id);
        Assert.Equal("updated", loaded.ExtraProperties[Defined]);
        Assert.Equal("preserve", loaded.Properties["existingJson"].GetString());
        Assert.Equal(42, loaded.Properties["newJson"].GetInt32());
    }

    [Fact]
    public async Task Scope_Only_Returns_Defined_Extensions_And_Round_Trips_Json_Properties()
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
        update.Properties["existingJson"] = JsonSerializer.SerializeToElement("preserve");
        update.Properties["newJson"] = JsonSerializer.SerializeToElement(true);
        update.ExtraProperties[Defined] = "updated";
        await service.UpdateAsync(created.Id, update);

        var loaded = await service.GetAsync(created.Id);
        Assert.Equal("updated", loaded.ExtraProperties[Defined]);
        Assert.Equal("preserve", loaded.Properties["existingJson"].GetString());
        Assert.True(loaded.Properties["newJson"].GetBoolean());
    }

    [Theory]
    [InlineData(false, "omitted")]
    [InlineData(false, "null")]
    [InlineData(false, "empty")]
    [InlineData(false, "replace")]
    [InlineData(true, "omitted")]
    [InlineData(true, "null")]
    [InlineData(true, "empty")]
    [InlineData(true, "replace")]
    public async Task Json_Updates_Distinguish_Omitted_Null_Empty_And_Replacement_Property_Bags(bool scope, string scenario)
    {
        var original = new Dictionary<string, JsonElement>
        {
            ["remove"] = JsonSerializer.SerializeToElement("old"),
            ["keep"] = JsonSerializer.SerializeToElement("before")
        };
        var propertiesJson = scenario switch
        {
            "null" => ",\"properties\":null",
            "empty" => ",\"properties\":{}",
            "replace" => ",\"properties\":{\"keep\":\"after\"}",
            _ => ""
        };
        var serializer = GetRequiredService<IJsonSerializer>();
        Dictionary<string, JsonElement> loaded;
        if (scope)
        {
            var service = GetRequiredService<IOpenIddictScopeAppService>();
            var created = await service.CreateAsync(new CreateOpenIddictScopeDto { Name = "replace-scope", Properties = original });
            var update = serializer.Deserialize<UpdateOpenIddictScopeDto>("{\"name\":\"replace-scope\"" + propertiesJson + "}");
            await service.UpdateAsync(created.Id, update);
            loaded = (await service.GetAsync(created.Id)).Properties;
        }
        else
        {
            var service = GetRequiredService<IOpenIddictApplicationAppService>();
            var input = ApplicationInput();
            input.Properties = original;
            var created = await service.CreateAsync(input);
            var update = serializer.Deserialize<CreateOrUpdateOpenIddictApplicationDto>(
                "{\"clientId\":\"extension-client\",\"clientType\":\"public\",\"consentType\":\"explicit\"" + propertiesJson + "}");
            await service.UpdateAsync(created.Id, update);
            loaded = (await service.GetAsync(created.Id)).Properties;
        }

        if (scenario is "omitted" or "null")
        {
            Assert.Equal(2, loaded.Count);
            Assert.Equal("old", loaded["remove"].GetString());
            Assert.Equal("before", loaded["keep"].GetString());
        }
        else if (scenario == "empty")
        {
            Assert.Empty(loaded);
        }
        else
        {
            Assert.Single(loaded);
            Assert.Equal("after", loaded["keep"].GetString());
        }
    }

    [Theory]
    [InlineData("omitted", false)]
    [InlineData("null", false)]
    [InlineData("replace", false)]
    [InlineData("omitted", true)]
    [InlineData("null", true)]
    [InlineData("replace", true)]
    public async Task BuildModel_Overrides_Can_Add_Server_Properties_With_Or_Without_A_Client_Bag(string scenario, bool mutateInput)
    {
        var service = GetRequiredService<IOpenIddictApplicationAppService>();
        var input = ApplicationInput();
        input.Properties["existing"] = JsonSerializer.SerializeToElement("old");
        var created = await service.CreateAsync(input);
        var custom = new ServerPropertiesApplicationService(ServiceProvider)
        {
            LazyServiceProvider = new AbpLazyServiceProvider(ServiceProvider),
            MutateInput = mutateInput
        };
        var update = ApplicationInput();
        var supplyClientBag = scenario == "replace";
        if (supplyClientBag)
        {
            update.Properties = new() { ["client"] = JsonSerializer.SerializeToElement("new") };
        }
        else if (scenario == "null")
        {
            update.Properties = null;
        }
        using (var unit = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false))
        {
            await custom.UpdateAsync(created.Id, update);
            await unit.CompleteAsync();
        }

        var loaded = await service.GetAsync(created.Id);
        Assert.Equal("trusted", loaded.Properties["server"].GetString());
        Assert.Equal(!supplyClientBag, loaded.Properties.ContainsKey("existing"));
        Assert.Equal(supplyClientBag, loaded.Properties.ContainsKey("client"));
    }

    [DisableConventionalRegistration]
    private sealed class ServerPropertiesApplicationService(IServiceProvider services) : OpenIddictApplicationAppService(
        services.GetRequiredService<IOpenIddictApplicationStore<OpenIddictApplicationModel>>(),
        services.GetRequiredService<IAbpApplicationManager>(),
        services.GetRequiredService<IPermissionDataSeeder>(),
        services.GetRequiredService<IPermissionManager>(),
        services.GetRequiredService<IOpenIddictApplicationRepository>(),
        services.GetRequiredService<IStringLocalizer<OpenIddictResponse>>())
    {
        public bool MutateInput { get; init; }

        protected override async Task<OpenIddictApplicationModel> BuildModel(CreateOrUpdateOpenIddictApplicationDto input)
        {
            if (MutateInput)
            {
                input.Properties ??= new();
                input.Properties["server"] = JsonSerializer.SerializeToElement("trusted");
                return await base.BuildModel(input);
            }
            var model = await base.BuildModel(input);
            var properties = await OpenIdApplicationStore.GetPropertiesAsync(model, CancellationToken.None);
            await OpenIdApplicationStore.SetPropertiesAsync(model,
                properties.SetItem("server", JsonSerializer.SerializeToElement("trusted")), CancellationToken.None);
            return model;
        }
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
