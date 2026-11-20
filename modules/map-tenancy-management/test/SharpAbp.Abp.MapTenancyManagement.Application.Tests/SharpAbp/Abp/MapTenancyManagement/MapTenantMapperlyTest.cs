using System;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectMapping;
using Volo.Abp.TenantManagement;
using Xunit;

namespace SharpAbp.Abp.MapTenancyManagement;

public class MapTenantMapperlyTest : MapTenancyManagementApplicationTestBase
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Map_Tenant_Maps_Identifiers_Codes_And_Defined_Extra_Properties(bool existing)
    {
        const string property = "MapTenantMapperPublic";
        ObjectExtensionManager.Instance.AddOrUpdateProperty<MapTenant, string>(property);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<MapTenantDto, string>(property);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<HybridMapTenantDto, string>(property);
        var source = new MapTenant(Guid.NewGuid(), Guid.NewGuid(), "tenant", "code", "map-code");
        source.SetProperty(property, "public");
        source.SetProperty("MapTenantMapperPrivate", "private");
        var mapper = GetRequiredService<IObjectMapper<MapTenancyManagementApplicationModule>>();

        var dto = existing ? mapper.Map(source, new MapTenantDto()) : mapper.Map<MapTenant, MapTenantDto>(source);
        var hybrid = existing ? mapper.Map(source, new HybridMapTenantDto()) : mapper.Map<MapTenant, HybridMapTenantDto>(source);

        Assert.Equal(source.Id, dto.Id);
        Assert.Equal(source.TenantId, dto.TenantId);
        Assert.Equal(source.TenantName, dto.TenantName);
        Assert.Equal(source.Code, dto.Code);
        Assert.Equal(source.MapCode, dto.MapCode);
        Assert.Equal(source.Id, hybrid.Id);
        Assert.Equal(source.TenantId, hybrid.TenantId);
        Assert.Equal(source.TenantName, hybrid.TenantName);
        Assert.Equal(source.Code, hybrid.Code);
        Assert.Equal(source.MapCode, hybrid.MapCode);
        Assert.Equal(source.ConcurrencyStamp, hybrid.ConcurrencyStamp);
        Assert.Equal("public", dto.ExtraProperties[property]);
        Assert.Equal("public", hybrid.ExtraProperties[property]);
        Assert.False(dto.ExtraProperties.ContainsKey("MapTenantMapperPrivate"));
        Assert.False(hybrid.ExtraProperties.ContainsKey("MapTenantMapperPrivate"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Tenant_To_Hybrid_Maps_Tenant_Identity_And_Preserves_Ignored_Mapping_Fields(bool existing)
    {
        Tenant source = new TestTenant(Guid.NewGuid(), "tenant");
        var mapper = GetRequiredService<IObjectMapper<MapTenancyManagementApplicationModule>>();
        var destination = new HybridMapTenantDto { TenantName = "mapped-name", Code = "code", MapCode = "map" };

        var result = existing ? mapper.Map(source, destination) : mapper.Map<Tenant, HybridMapTenantDto>(source);

        Assert.Equal(source.Id, result.TenantId);
        Assert.Equal(source.ConcurrencyStamp, result.ConcurrencyStamp);
        Assert.Equal(existing ? "mapped-name" : null, result.TenantName);
        Assert.Equal(existing ? "code" : null, result.Code);
        Assert.Equal(existing ? "map" : null, result.MapCode);
    }

    private sealed class TestTenant(Guid id, string name) : Tenant(id, name, name.ToUpperInvariant());
}
