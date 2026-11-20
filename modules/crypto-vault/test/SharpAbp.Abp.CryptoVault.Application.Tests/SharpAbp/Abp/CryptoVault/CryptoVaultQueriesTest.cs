using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.CryptoVault;

public class CryptoVaultQueriesTest : AbpCryptoVaultApplicationTestBase
{
    [Fact]
    public async Task Rsa_Paging_And_Identifier_Queries_Return_Only_Matching_Key()
    {
        var service = new RSACredsController(GetRequiredService<IRSACredsAppService>());
        await service.GenerateAsync(new GenerateRSACredsDto { Count = 2, Size = 2048 });
        var keys = await service.GetListAsync(size: 2048);
        Assert.Equal(2, keys.Count);
        var key = keys[0];

        var found = await service.FindByIdentifierAsync(key.Identifier);
        var page = await service.GetPagedListAsync(new RSACredsPagedRequestDto
        {
            Identifier = key.Identifier, SourceType = key.SourceType, Size = 2048, MaxResultCount = 1
        });

        Assert.Equal(key.Id, found.Id);
        Assert.Equal(1, page.TotalCount);
        Assert.Equal(key.Id, Assert.Single(page.Items).Id);
        Assert.Empty(await service.GetListAsync(identifier: "missing", sourceType: key.SourceType, size: 2048));
        var decrypted = await service.GetDecryptKey(key.Id);
        Assert.NotEmpty(decrypted.PublicKey);
        Assert.NotEmpty(decrypted.PrivateKey);
        Assert.NotEqual(key.PrivateKey, decrypted.PrivateKey);
    }

    [Fact]
    public async Task Sm2_Paging_And_Identifier_Queries_Return_Only_Matching_Key()
    {
        var service = new SM2CredsController(GetRequiredService<ISM2CredsAppService>());
        await service.GenerateAsync(new GenerateSM2CredsDto { Count = 2, Curve = "sm2p256v1" });
        var keys = await service.GetListAsync(curve: "sm2p256v1");
        Assert.Equal(2, keys.Count);
        var key = keys[0];

        var found = await service.FindByIdentifierAsync(key.Identifier);
        var page = await service.GetPagedListAsync(new SM2CredsPagedRequestDto
        {
            Identifier = key.Identifier, SourceType = key.SourceType, Curve = key.Curve, MaxResultCount = 1
        });

        Assert.Equal(key.Id, found.Id);
        Assert.Equal(1, page.TotalCount);
        Assert.Equal(key.Id, Assert.Single(page.Items).Id);
        Assert.Empty(await service.GetListAsync(identifier: "missing", sourceType: key.SourceType, curve: key.Curve));
        var decrypted = await service.GetDecryptKey(key.Id);
        Assert.NotEmpty(decrypted.PublicKey);
        Assert.NotEmpty(decrypted.PrivateKey);
        Assert.NotEqual(key.PrivateKey, decrypted.PrivateKey);
    }
}
