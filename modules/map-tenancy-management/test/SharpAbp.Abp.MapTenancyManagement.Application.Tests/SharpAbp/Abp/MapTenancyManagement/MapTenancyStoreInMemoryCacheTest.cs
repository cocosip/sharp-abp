using System;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.MapTenancyManagement;

public class MapTenancyStoreInMemoryCacheTest
{
    [Fact]
    public async Task Filling_Replaces_Entries_And_Copies_Entity_Values()
    {
        var cache = new MapTenancyStoreInMemoryCache();
        var first = new MapTenant(Guid.NewGuid(), Guid.NewGuid(), "first", "code-one", "map-one");
        var second = new MapTenant(Guid.NewGuid(), Guid.NewGuid(), "second", "code-two", "map-two");
        await cache.FillAsync([first]);

        var cached = cache.GetByTenantIdOrNull(first.TenantId);
        Assert.Equal("first", cached.TenantName);
        Assert.Same(cached, cache.GetByCodeOrNull("code-one"));
        Assert.Same(cached, cache.GetByMapCodeOrNull("map-one"));
        first.Code = "changed";
        Assert.Equal("code-one", cached.Code);

        await cache.FillAsync([second]);

        Assert.Null(cache.GetByTenantIdOrNull(first.TenantId));
        Assert.Null(cache.GetByCodeOrNull("code-one"));
        Assert.Null(cache.GetByMapCodeOrNull("map-one"));
        Assert.Equal(second.TenantId, Assert.Single(cache.GetAll()).TenantId);
        await cache.FillAsync([]);
        Assert.Empty(cache.GetAll());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Lookups_Reject_Empty_Codes(string code)
    {
        var cache = new MapTenancyStoreInMemoryCache();

        Assert.ThrowsAny<ArgumentException>(() => cache.GetByCodeOrNull(code));
        Assert.ThrowsAny<ArgumentException>(() => cache.GetByMapCodeOrNull(code));
    }

    [Fact]
    public void Cache_Item_Maps_All_Fields_And_Rejects_Null_Entity()
    {
        var entity = new MapTenant(Guid.NewGuid(), Guid.NewGuid(), "tenant", "code", "map");

        var item = entity.AsCacheItem();

        Assert.Equal(entity.TenantId, item.TenantId);
        Assert.Equal(entity.TenantName, item.TenantName);
        Assert.Equal(entity.Code, item.Code);
        Assert.Equal(entity.MapCode, item.MapCode);
        Assert.Throws<ArgumentNullException>(() => MapTenantExtensions.AsCacheItem(null));
    }
}
