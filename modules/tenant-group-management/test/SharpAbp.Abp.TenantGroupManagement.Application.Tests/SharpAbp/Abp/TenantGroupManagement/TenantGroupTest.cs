using System;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.TenantGroupManagement;

public class TenantGroupTest
{
    [Fact]
    public void Tenant_Membership_Rejects_Duplicates_And_Removes_By_Both_Identifiers()
    {
        var group = new TenantGroup(Guid.NewGuid(), "group", "GROUP", true);
        var first = new TenantGroupTenant(Guid.NewGuid(), group.Id, Guid.NewGuid());
        var second = new TenantGroupTenant(Guid.NewGuid(), group.Id, Guid.NewGuid());
        group.AddTenant(first);
        group.AddTenant(second);

        Assert.Throws<AbpException>(() => group.AddTenant(new TenantGroupTenant(Guid.NewGuid(), group.Id, first.TenantId)));
        Assert.Equal(2, group.Tenants.Count);
        group.RemoveTenant(Guid.NewGuid());
        group.RemoveTenantByTenantId(Guid.NewGuid());
        Assert.Equal(2, group.Tenants.Count);
        group.RemoveTenant(first.Id);
        Assert.Same(second, Assert.Single(group.Tenants));
        group.RemoveTenantByTenantId(second.TenantId);
        Assert.Empty(group.Tenants);
    }

    [Fact]
    public void Connection_Strings_Update_In_Place_And_Remove_Independently()
    {
        var group = new TenantGroup(Guid.NewGuid(), "group", "GROUP", true);
        Assert.Null(group.FindConnectionString("Reporting"));
        Assert.Null(group.GetDefaultConnectionString());
        group.SetDefaultConnectionString("default-one");
        group.SetConnectionString("Reporting", "report-one");
        var original = group.GetConnectionString("Reporting");

        group.SetConnectionString("Reporting", "report-two");

        Assert.Same(original, group.GetConnectionString("Reporting"));
        Assert.Equal("report-two", group.FindConnectionString("Reporting"));
        Assert.Equal("default-one", group.FindDefaultConnectionString());
        Assert.Equal(2, group.ConnectionStrings.Count);
        group.RemoveConnectionString("missing");
        group.RemoveDefaultConnectionString();
        Assert.Null(group.FindDefaultConnectionString());
        Assert.Equal("Reporting", Assert.Single(group.ConnectionStrings).Name);
        group.RemoveConnectionString("Reporting");
        Assert.Empty(group.ConnectionStrings);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_Rejects_Missing_Name(string name)
    {
        Assert.ThrowsAny<ArgumentException>(() => new TenantGroup(Guid.NewGuid(), name, null, true));
    }
}
