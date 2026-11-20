using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.MinId;

public class MinIdTokenCacheTransactionTest : MinIdApplicationTestBase
{
    [Theory]
    [InlineData("create", true)]
    [InlineData("create", false)]
    [InlineData("update", true)]
    [InlineData("update", false)]
    [InlineData("delete", true)]
    [InlineData("delete", false)]
    public async Task Token_Changes_Invalidate_Shared_Cache_Only_After_Commit(string operation, bool commit)
    {
        var service = GetRequiredService<IMinIdTokenAppService>();
        var cache = GetRequiredService<IDistributedCache<MinIdTokenCacheItem>>();
        var manager = GetRequiredService<IUnitOfWorkManager>();
        MinIdTokenDto existing = null;
        if (operation != "create")
        {
            existing = await service.CreateAsync(new CreateMinIdTokenDto
            {
                BizType = "default", Token = "transactiontoken", Remark = "cache transaction test"
            });
        }

        string[] tokens = operation == "update" ? ["transactiontoken", "replacementtoken"] : ["transactiontoken"];
        foreach (var token in tokens)
        {
            await cache.SetAsync($"default-{token}", new MinIdTokenCacheItem { BizType = "default", Token = token });
        }

        using (var unitOfWork = manager.Begin(requiresNew: true, isTransactional: true))
        {
            if (operation == "create")
            {
                await service.CreateAsync(new CreateMinIdTokenDto
                {
                    BizType = "default", Token = "transactiontoken", Remark = "cache transaction test"
                });
            }
            else if (operation == "update")
            {
                await service.UpdateAsync(existing.Id, new UpdateMinIdTokenDto
                {
                    BizType = "default", Token = "replacementtoken", Remark = "cache transaction test"
                });
            }
            else
            {
                await service.DeleteAsync(existing.Id);
            }

            foreach (var token in tokens)
            {
                Assert.NotNull(await cache.GetAsync($"default-{token}"));
            }

            if (commit)
            {
                await unitOfWork.CompleteAsync();
            }
            else
            {
                await unitOfWork.RollbackAsync();
            }
        }

        foreach (var token in tokens)
        {
            var cached = await cache.GetAsync($"default-{token}");
            if (commit)
            {
                Assert.Null(cached);
            }
            else
            {
                Assert.NotNull(cached);
                Assert.Equal(token, cached.Token);
            }
        }
    }
}
