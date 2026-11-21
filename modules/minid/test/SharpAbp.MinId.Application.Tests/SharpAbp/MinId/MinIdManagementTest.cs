using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace SharpAbp.MinId;

public class MinIdManagementTest : MinIdApplicationTestBase
{
    [Fact]
    public async Task Seeding_Does_Not_Recreate_A_Retired_Default()
    {
        var service = GetRequiredService<IMinIdInfoAppService>();
        var existing = await service.FindByBizTypeAsync("default");

        await service.DeleteAsync(existing.Id);
        await GetRequiredService<IDataSeeder>().SeedAsync();

        Assert.Empty(await service.GetListAsync());
    }

    [Fact]
    public async Task Configuration_Cannot_Lower_The_Watermark_Or_Reuse_A_Retired_Name()
    {
        var service = GetRequiredService<IMinIdInfoAppService>();
        var input = new CreateMinIdInfoDto { BizType = "billing", MaxId = 100, Step = 10, Delta = 1 };
        var created = await service.CreateAsync(input);
        await Assert.ThrowsAsync<UserFriendlyException>(() => service.UpdateAsync(created.Id,
            new UpdateMinIdInfoDto { BizType = input.BizType, MaxId = 99, Step = 10, Delta = 1 }));
        await Assert.ThrowsAsync<UserFriendlyException>(() => service.UpdateAsync(created.Id,
            new UpdateMinIdInfoDto { BizType = "renamed", MaxId = 100, Step = 10, Delta = 1 }));
        Assert.Equal(100, (await service.GetAsync(created.Id)).MaxId);

        await service.DeleteAsync(created.Id);

        Assert.Empty(await service.GetListAsync(bizType: input.BizType));
        await Assert.ThrowsAsync<UserFriendlyException>(() => service.CreateAsync(input));
    }

    [Fact]
    public async Task Configuration_Crud_Filters_And_Rejects_Duplicate_Business_Type()
    {
        var service = new MinIdInfoController(GetRequiredService<IMinIdInfoAppService>());
        var input = new CreateMinIdInfoDto { BizType = "orders", MaxId = 100, Step = 10, Delta = 1, Remainder = 0 };
        var created = await service.CreateAsync(input);
        await Assert.ThrowsAsync<UserFriendlyException>(() => service.CreateAsync(input));

        var found = await service.FindByBizTypeAsync("orders");
        Assert.Equal(created.Id, found.Id);
        Assert.Equal(100, (await service.GetAsync(created.Id)).MaxId);
        Assert.Equal(created.Id, Assert.Single(await service.GetListAsync(bizType: "orders")).Id);
        var page = await service.GetPagedListAsync(new MinIdInfoPagedRequestDto { BizType = "orders" });
        Assert.Equal(1, page.TotalCount);
        Assert.Equal(created.Id, Assert.Single(page.Items).Id);

        var updated = await service.UpdateAsync(created.Id, new UpdateMinIdInfoDto
        {
            BizType = "orders", MaxId = 200, Step = 20, Delta = 2, Remainder = 1
        });
        Assert.Equal("orders", updated.BizType);
        Assert.Equal(200, updated.MaxId);
        Assert.Equal(20, updated.Step);
        Assert.Equal(2, updated.Delta);
        Assert.Equal(1, updated.Remainder);
        await service.DeleteAsync(created.Id);
        Assert.Empty(await service.GetListAsync(bizType: "orders"));
        await Assert.ThrowsAnyAsync<EntityNotFoundException>(() => service.GetAsync(created.Id));
    }

    [Fact]
    public async Task Token_Queries_And_Deletion_Invalidate_Cached_Validation()
    {
        var service = new MinIdTokenController(GetRequiredService<IMinIdTokenAppService>());
        var validator = GetRequiredService<IMinIdTokenValidator>();
        await new MinIdInfoController(GetRequiredService<IMinIdInfoAppService>()).CreateAsync(new CreateMinIdInfoDto
        {
            BizType = "orders", Step = 10, Delta = 1
        });
        var created = await service.CreateAsync(new CreateMinIdTokenDto { BizType = "orders", Token = "token123", Remark = "test" });

        Assert.Equal(created.Id, (await service.GetAsync(created.Id)).Id);
        Assert.Equal(created.Id, (await service.FindByTokenAsync("orders", "token123")).Id);
        Assert.Equal(created.Id, (await service.FindAsync(new FindMinIdTokenDto { BizType = "orders", Token = "token123" })).Id);
        var page = await service.GetPagedListAsync(new MinIdTokenPagedRequestDto { BizType = "orders", Token = "token123" });
        Assert.Equal(1, page.TotalCount);
        Assert.Equal(created.Id, Assert.Single(page.Items).Id);
        Assert.True(await validator.ValidateAsync("orders", "token123"));
        await validator.ValidateTokenAsync("orders", "token123");

        await service.DeleteAsync(created.Id);

        Assert.False(await validator.ValidateAsync("orders", "token123"));
        await Assert.ThrowsAnyAsync<AbpException>(() => validator.ValidateTokenAsync("orders", "token123"));
    }
}
