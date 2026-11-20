using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.MapTenancy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.MapTenancy;

public class MapTenancyConfigurationTest
{
    [Fact]
    public async Task Bound_Mappings_Should_Be_Resolved_By_Code_MapCode_And_Tenant()
    {
        var tenantId = Guid.NewGuid();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            ["MapTenancyOptions:local:TenantId"] = tenantId.ToString(),
            ["MapTenancyOptions:local:TenantName"] = "Acme",
            ["MapTenancyOptions:local:Code"] = "local",
            ["MapTenancyOptions:local:MapCode"] = "partner"
        }).Build();
        var options = new AbpMapTenancyOptions().Configure(configuration);
        var provider = new DefaultMapTenancyConfigurationProvider(Options.Create(options));
        var codeProvider = new DefaultMapTenantCodeProvider(Options.Create(options));

        var byCode = await provider.GetAsync("local");
        Assert.Same(byCode, await provider.GetByMapCodeAsync("partner"));
        Assert.Equal(tenantId, byCode.TenantId);
        var resolved = await codeProvider.FindByTenantIdAsync(tenantId);
        Assert.NotNull(resolved);
        Assert.Equal(tenantId, resolved.TenantId);
        Assert.Equal("Acme", resolved.TenantName);
        Assert.Equal("local", resolved.Code);
        Assert.Equal("partner", resolved.MapCode);
        Assert.Null(await codeProvider.FindByTenantIdAsync(Guid.NewGuid()));
        Assert.Null(await provider.GetByMapCodeAsync("missing"));
    }

    [Fact]
    public void Reconfiguration_Should_Update_Existing_Mapping_And_Apply_To_All()
    {
        var mappings = new MapTenancyConfigurations();
        mappings.Configure("first", mapping => mapping.Code = "first");
        var first = mappings.GetConfiguration("first");
        Assert.Same(mappings, mappings.Configure("first", mapping => mapping.MapCode = "partner"));
        mappings.Configure("second", mapping => mapping.Code = "second");

        Assert.Same(mappings, mappings.ConfigureAll((code, mapping) => mapping.TenantName = code));

        Assert.Same(first, mappings.GetConfiguration("first"));
        Assert.Equal("partner", first.MapCode);
        Assert.Equal("first", first.TenantName);
        Assert.Equal("second", mappings.GetConfiguration("second").TenantName);
        var missing = mappings.GetConfiguration("missing");
        missing.Code = "changed";
        Assert.Null(mappings.GetConfiguration("missing").Code);
    }

    [Fact]
    public async Task Tenant_Without_Local_Code_Should_Not_Produce_Code_Info()
    {
        var tenantId = Guid.NewGuid();
        var options = new AbpMapTenancyOptions().Configure(new ConfigurationBuilder().Build());
        options.Mappers.Configure("incomplete", mapping => mapping.TenantId = tenantId);

        var result = await new DefaultMapTenantCodeProvider(Options.Create(options)).FindByTenantIdAsync(tenantId);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Mapping_Lookups_Should_Reject_Empty_Keys(string code)
    {
        var mappings = new MapTenancyConfigurations();
        Assert.ThrowsAny<ArgumentException>(() => mappings.Configure(code, _ => { }));
        Assert.ThrowsAny<ArgumentException>(() => mappings.GetConfiguration(code));
        Assert.ThrowsAny<ArgumentException>(() => mappings.GetConfigurationByMapCode(code));
    }

    [Fact]
    public void Mapping_Configuration_Should_Reject_Missing_Callback()
    {
        Assert.Throws<ArgumentNullException>(() => new MapTenancyConfigurations().Configure("local", null));
    }
}
