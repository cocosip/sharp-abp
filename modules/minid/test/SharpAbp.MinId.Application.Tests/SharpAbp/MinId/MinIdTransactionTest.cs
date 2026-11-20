using System;
using System.Threading.Tasks;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.MinId;

public class MinIdTransactionTest : MinIdApplicationTestBase
{
    [Fact]
    public async Task Disposing_Uncompleted_Transaction_Rolls_Back_Saved_Configuration()
    {
        var manager = GetRequiredService<IUnitOfWorkManager>();
        var repository = GetRequiredService<IMinIdInfoRepository>();
        using (var unitOfWork = manager.Begin(requiresNew: true, isTransactional: true))
        {
            await repository.InsertAsync(new MinIdInfo(Guid.NewGuid(), "rollback", 0, 10, 1, 0), autoSave: true);
            Assert.NotNull(await repository.FindByBizTypeAsync("rollback"));
        }

        using var verification = manager.Begin(requiresNew: true, isTransactional: true);
        Assert.Null(await repository.FindByBizTypeAsync("rollback"));
    }
}
