using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using SharpAbp.MinId.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.MinId;

public class MinIdTokenValidatorTest : MinIdApplicationTestBase
{
    [Fact]
    public async Task Validation_Respects_Database_Business_Type_Collation_While_Keeping_Tokens_Case_Sensitive()
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var context = await GetRequiredService<IDbContextProvider<MinIdDbContext>>().GetDbContextAsync();
        var tableName = context.Model.FindEntityType(typeof(MinIdToken)).GetTableName();
        var createTable = context.Database.GenerateCreateScript().Split(';')
            .Single(statement => statement.Contains($"CREATE TABLE \"{tableName}\""))
            .Replace("\"BizType\" TEXT", "\"BizType\" TEXT COLLATE NOCASE")
            .Replace("\"Token\" TEXT", "\"Token\" TEXT COLLATE NOCASE");
        await context.Database.OpenConnectionAsync();
        using (var command = context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "DROP TABLE " + context.GetService<ISqlGenerationHelper>().DelimitIdentifier(tableName);
            await command.ExecuteNonQueryAsync();
        }
        await context.Database.ExecuteSqlRawAsync(createTable);
        var repository = GetRequiredService<IMinIdTokenRepository>();
        await repository.InsertAsync(new MinIdToken(Guid.NewGuid(), "Orders", "CaseSensitiveToken", "collation test"), autoSave: true);

        Assert.NotNull(await repository.FindByTokenAsync("ORDERS", "CaseSensitiveToken"));
        var validator = GetRequiredService<IMinIdTokenValidator>();
        Assert.True(await validator.ValidateAsync("ORDERS", "CaseSensitiveToken"));
        Assert.False(await validator.ValidateAsync("ORDERS", "casesensitivetoken"));
    }

    [Fact]
    public async Task Revocation_Is_Not_Undone_By_A_Late_Cache_Write()
    {
        const string token = "revocationtoken";
        var service = GetRequiredService<IMinIdTokenAppService>();
        var validator = GetRequiredService<IMinIdTokenValidator>();
        var created = await service.CreateAsync(new CreateMinIdTokenDto
        {
            BizType = "default", Token = token, Remark = "revocation test"
        });
        Assert.True(await validator.ValidateAsync("default", token));
        await service.DeleteAsync(created.Id);

        await GetRequiredService<IDistributedCache<MinIdTokenCacheItem>>().SetAsync($"default-{token}",
            new MinIdTokenCacheItem { BizType = "default", Token = token });

        Assert.False(await validator.ValidateAsync("default", token));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Invalid_Or_Mismatched_Cached_Token_Is_Rejected_Without_Disclosing_Secret(bool mismatchedCache)
    {
        const string token = "submittedsecrettoken";
        var validator = GetRequiredService<IMinIdTokenValidator>();
        if (mismatchedCache)
        {
            await GetRequiredService<IDistributedCache<MinIdTokenCacheItem>>().SetAsync($"default-{token}",
                new MinIdTokenCacheItem { BizType = "default", Token = "differenttoken" });
        }

        Assert.False(await validator.ValidateAsync("default", token));
        var error = await Assert.ThrowsAnyAsync<AbpException>(() => validator.ValidateTokenAsync("default", token));
        Assert.DoesNotContain(token, error.Message);
        Assert.Contains("Invalid token", error.Message);
        Assert.Contains("default", error.Message);

        var controller = new IdController(validator,
            GetRequiredService<IMinIdGeneratorAppService>(), GetRequiredService<ISegmentIdAppService>());
        var response = await controller.NextIdAsync("default", "Bearer " + token);
        Assert.False(response.Success);
        Assert.DoesNotContain(token, response.Message);
        Assert.Contains("Invalid token", response.Message);
    }
}
