using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace SharpAbp.MinId;

public class MinIdManagementTest : MinIdApplicationTestBase
{
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
            BizType = "invoices", MaxId = 200, Step = 20, Delta = 2, Remainder = 1
        });
        Assert.Equal("invoices", updated.BizType);
        Assert.Equal(200, updated.MaxId);
        Assert.Equal(20, updated.Step);
        Assert.Equal(2, updated.Delta);
        Assert.Equal(1, updated.Remainder);
        Assert.Empty(await service.GetListAsync(bizType: "orders"));
        await service.DeleteAsync(created.Id);
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
        var page = await service.GetPagedListAsync(new MinIdTokenPagedRequestDto { BizType = "orders", Token = "token123" });
        Assert.Equal(1, page.TotalCount);
        Assert.Equal(created.Id, Assert.Single(page.Items).Id);
        Assert.True(await validator.ValidateAsync("orders", "token123"));
        await validator.ValidateTokenAsync("orders", "token123");

        await service.DeleteAsync(created.Id);

        Assert.False(await validator.ValidateAsync("orders", "token123"));
        await Assert.ThrowsAsync<AbpException>(() => validator.ValidateTokenAsync("orders", "token123"));
    }
}
