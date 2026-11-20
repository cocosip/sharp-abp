using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.CryptoVault.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.Abp.CryptoVault;

public class CryptoVaultRandomSelectionRaceTest : AbpIntegratedTest<CryptoVaultRandomSelectionRaceTestModule>
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options) => options.UseAutofac();

    [Fact]
    public async Task Rsa_Falls_Back_To_Surviving_Match_When_Random_Offset_Returns_No_Row()
    {
        var repository = GetRequiredService<IRSACredsRepository>();
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin();
        var expected = new RSACreds(Guid.NewGuid(), "remaining", 1, 2048, "public", "private", "pass", "salt", "test");
        await repository.InsertAsync(expected, autoSave: true);
        GetRequiredService<EmptyOffsetResultInterceptor>().SuppressNextOffsetQuery = true;

        var result = await repository.GetRandomAsync(sourceType: 1, size: 2048);

        Assert.NotNull(result);
        Assert.Equal(expected.Id, result.Id);
        Assert.False(GetRequiredService<EmptyOffsetResultInterceptor>().SuppressNextOffsetQuery);
    }

    [Fact]
    public async Task Sm2_Falls_Back_To_Surviving_Match_When_Random_Offset_Returns_No_Row()
    {
        var repository = GetRequiredService<ISM2CredsRepository>();
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin();
        var expected = new SM2Creds(Guid.NewGuid(), "remaining", 1, "sm2p256v1", "public", "private", "pass", "salt", "test");
        await repository.InsertAsync(expected, autoSave: true);
        GetRequiredService<EmptyOffsetResultInterceptor>().SuppressNextOffsetQuery = true;

        var result = await repository.GetRandomAsync(sourceType: 1, curve: "sm2p256v1");

        Assert.NotNull(result);
        Assert.Equal(expected.Id, result.Id);
        Assert.False(GetRequiredService<EmptyOffsetResultInterceptor>().SuppressNextOffsetQuery);
    }
}

[DependsOn(typeof(AbpCryptoVaultApplicationTestModule))]
public class CryptoVaultRandomSelectionRaceTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var interceptor = new EmptyOffsetResultInterceptor();
        context.Services.AddSingleton(interceptor);
        Configure<AbpDbContextOptions>(options => options.Configure<AbpCryptoVaultDbContext>(configuration =>
        {
            configuration.UseSqlite();
            configuration.DbContextOptions.AddInterceptors(interceptor);
        }));
    }
}

public class EmptyOffsetResultInterceptor : DbCommandInterceptor
{
    public bool SuppressNextOffsetQuery { get; set; }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
        CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
    {
        if (SuppressNextOffsetQuery && command.CommandText.Contains("OFFSET", StringComparison.Ordinal))
        {
            SuppressNextOffsetQuery = false;
            // Model a stale count without depending on which random offset was chosen.
            command.CommandText = $"SELECT * FROM ({command.CommandText}) WHERE 0";
        }
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }
}
