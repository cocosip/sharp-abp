using SharpAbp.Abp.TenancyGrouping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.TenancyGrouping;

public class TenantGroupScopeTest
{
    [Fact]
    public async Task Nested_Changes_Should_Restore_Parent_Across_Await_And_Isolate_Child_Tasks()
    {
        var current = new CurrentTenantGroup(AsyncLocalCurrentTenantGroupAccessor.Instance);
        var outerId = Guid.NewGuid();
        var innerId = Guid.NewGuid();
        var tenants = new List<Guid> { Guid.NewGuid() };
        using (current.Change(outerId, "outer", tenants))
        {
            Assert.True(current.IsAvailable);
            Assert.Equal(outerId, current.GetId());
            Assert.Equal("outer", current.Name);
            Assert.Same(tenants, current.Tenants);
            await Task.Run(async () =>
            {
                using (current.Change(innerId, "inner"))
                {
                    await Task.Yield();
                    Assert.Equal(innerId, current.GetId());
                }
                Assert.Equal(outerId, current.GetId());
            });
            Assert.Equal(outerId, current.GetId());
            using (current.Change(null))
            {
                Assert.False(current.IsAvailable);
                Assert.Throws<AbpException>(() => current.GetId());
            }
            Assert.Equal(outerId, current.GetId());
        }
        Assert.False(current.IsAvailable);
        Assert.Null(current.Name);
        Assert.Null(current.Tenants);
    }

    [Fact]
    public void Tenant_Membership_Should_Support_Empty_And_Existing_Collections()
    {
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();
        var info = new BasicTenantGroupInfo(Guid.NewGuid());
        info.RemoveTenant(first);
        info.ClearTenant();
        info.AddTenant(first);
        info.AddTenants([second]);
        Assert.Equal(new[] { first, second }, info.Tenants);
        info.RemoveTenant(first);
        Assert.Equal(new[] { second }, info.Tenants);
        info.ClearTenant();
        Assert.Empty(info.Tenants);
        var fromRange = new BasicTenantGroupInfo(null);
        fromRange.AddTenants([first, second]);
        Assert.Equal(new[] { first, second }, fromRange.Tenants);
    }

    [Fact]
    public void Cache_Keys_Should_Separate_Id_And_Name_And_Reject_Missing_Identity()
    {
        var id = Guid.Parse("d08c01ad-d579-4ced-aaf4-30c075133daa");
        Assert.Equal("i:d08c01ad-d579-4ced-aaf4-30c075133daa,n:null", TenantGroupConfigurationCacheItem.CalculateCacheKey(id));
        Assert.Equal("i:null,n:ACME", TenantGroupConfigurationCacheItem.CalculateCacheKey("ACME"));
        Assert.Equal("i:d08c01ad-d579-4ced-aaf4-30c075133daa,n:ACME", TenantGroupConfigurationCacheItem.CalculateCacheKey(id, "ACME"));
        Assert.Equal("i:d08c01ad-d579-4ced-aaf4-30c075133daa,n:null", TenantGroupConfigurationCacheItem.CalculateCacheKey(id, " "));
        Assert.Equal("i:null,n:ACME", TenantGroupConfigurationCacheItem.CalculateCacheKey(null, "ACME"));
        Assert.Throws<AbpException>(() => TenantGroupConfigurationCacheItem.CalculateCacheKey(null, " "));
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("acme", "ACME")]
    [InlineData("cafe\u0301", "CAFÉ")]
    public void Normalizer_Should_Use_Invariant_Unicode_Normalization(string input, string expected)
    {
        Assert.Equal(expected, new UpperInvariantTenantGroupNormalizer().NormalizeName(input));
    }
}
