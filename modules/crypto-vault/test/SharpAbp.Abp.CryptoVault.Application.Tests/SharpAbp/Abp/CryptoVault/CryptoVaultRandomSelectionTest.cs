using System;
using System.Threading.Tasks;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.Abp.CryptoVault;

public class CryptoVaultRandomSelectionTest : AbpCryptoVaultApplicationTestBase
{
    [Fact]
    public async Task Rsa_Random_Selection_Uses_Relational_Query_And_Respects_All_Filters()
    {
        var repository = GetRequiredService<IRSACredsRepository>();
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin();
        var expected = new RSACreds(Guid.NewGuid(), "matching", 1, 2048, "public", "private", "pass", "salt", "test");
        await repository.InsertManyAsync([
            expected,
            new RSACreds(Guid.NewGuid(), "wrong-source", 2, 2048, "public", "private", "pass", "salt", "test"),
            new RSACreds(Guid.NewGuid(), "wrong-size", 1, 3072, "public", "private", "pass", "salt", "test")
        ], autoSave: true);

        Assert.Equal(expected.Id, (await repository.GetRandomAsync(sourceType: 1, size: 2048)).Id);
        Assert.Null(await repository.GetRandomAsync(size: 4096));
        var any = await repository.GetRandomAsync();
        Assert.NotNull(any);
        Assert.Contains(any.Identifier, new[] { "matching", "wrong-source", "wrong-size" });
    }

    [Fact]
    public async Task Sm2_Random_Selection_Uses_Relational_Query_And_Respects_All_Filters()
    {
        var repository = GetRequiredService<ISM2CredsRepository>();
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin();
        var expected = new SM2Creds(Guid.NewGuid(), "matching", 1, "sm2p256v1", "public", "private", "pass", "salt", "test");
        await repository.InsertManyAsync([
            expected,
            new SM2Creds(Guid.NewGuid(), "wrong-source", 2, "sm2p256v1", "public", "private", "pass", "salt", "test"),
            new SM2Creds(Guid.NewGuid(), "wrong-curve", 1, "wapip192v1", "public", "private", "pass", "salt", "test")
        ], autoSave: true);

        Assert.Equal(expected.Id, (await repository.GetRandomAsync(sourceType: 1, curve: "sm2p256v1")).Id);
        Assert.Null(await repository.GetRandomAsync(curve: "missing"));
        Assert.NotNull(await repository.GetRandomAsync());
    }
}
