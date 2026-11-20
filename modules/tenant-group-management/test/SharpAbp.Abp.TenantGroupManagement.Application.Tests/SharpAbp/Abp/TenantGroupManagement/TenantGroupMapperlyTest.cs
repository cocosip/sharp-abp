using System;
using SharpAbp.Abp.TenancyGrouping;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace SharpAbp.Abp.TenantGroupManagement;

public class TenantGroupMapperlyTest : TenantGroupManagementTestBase
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Group_Maps_Nested_Connections_Tenants_And_Defined_Extra_Properties(bool existing)
    {
        const string property = "TenantGroupMapperPublic";
        ObjectExtensionManager.Instance.AddOrUpdateProperty<TenantGroup, string>(property);
        ObjectExtensionManager.Instance.AddOrUpdateProperty<TenantGroupDto, string>(property);
        var source = new TenantGroup(Guid.NewGuid(), "group", "GROUP", true);
        var tenant = new TenantGroupTenant(Guid.NewGuid(), source.Id, Guid.NewGuid());
        source.AddTenant(tenant);
        source.SetDefaultConnectionString("example-value");
        source.SetProperty(property, "public");
        source.SetProperty("TenantGroupMapperPrivate", "private");
        var mapper = GetRequiredService<IObjectMapper<TenantGroupManagementApplicationModule>>();

        var result = existing ? mapper.Map(source, new TenantGroupDto()) : mapper.Map<TenantGroup, TenantGroupDto>(source);

        Assert.Equal(source.Id, result.Id);
        Assert.Equal("group", result.Name);
        Assert.Equal("GROUP", result.NormalizedName);
        Assert.True(result.IsActive);
        Assert.Equal(source.ConcurrencyStamp, result.ConcurrencyStamp);
        var mappedTenant = Assert.Single(result.Tenants);
        Assert.Equal(tenant.Id, mappedTenant.Id);
        Assert.Equal(source.Id, mappedTenant.TenantGroupId);
        Assert.Equal(tenant.TenantId, mappedTenant.TenantId);
        var connection = Assert.Single(result.ConnectionStrings);
        Assert.Equal(source.Id, connection.TenantGroupId);
        Assert.Equal("Default", connection.Name);
        Assert.Equal("example-value", connection.Value);
        Assert.Equal("public", result.ExtraProperties[property]);
        Assert.False(result.ExtraProperties.ContainsKey("TenantGroupMapperPrivate"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Domain_Configuration_Replaces_Existing_Lists_And_Maps_Tenant_Ids(bool existing)
    {
        var source = new TenantGroup(Guid.NewGuid(), "group", "GROUP", true);
        var tenant = new TenantGroupTenant(Guid.NewGuid(), source.Id, Guid.NewGuid());
        source.AddTenant(tenant);
        source.SetDefaultConnectionString("example-value");
        var mapper = GetRequiredService<IObjectMapper<TenantGroupManagementDomainModule>>();
        var stale = new TenantGroupConfiguration { Tenants = [Guid.NewGuid()] };

        var result = existing ? mapper.Map(source, stale) : mapper.Map<TenantGroup, TenantGroupConfiguration>(source);
        var eto = existing ? mapper.Map(source, new TenantGroupEto()) : mapper.Map<TenantGroup, TenantGroupEto>(source);

        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Name, result.Name);
        Assert.Equal(tenant.TenantId, Assert.Single(result.Tenants));
        Assert.Equal("example-value", result.ConnectionStrings["Default"]);
        Assert.Equal(source.Id, eto.Id);
        Assert.Equal(tenant.TenantId, Assert.Single(eto.Tenants));
    }
}
