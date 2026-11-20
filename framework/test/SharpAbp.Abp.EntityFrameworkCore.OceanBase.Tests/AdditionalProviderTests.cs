using System;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Dm;
using GaussDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Data;
using SharpAbp.Abp.EntityFrameworkCore.ConnectionStrings;
using SharpAbp.Abp.EntityFrameworkCore.DM;
using SharpAbp.Abp.EntityFrameworkCore.GaussDB;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.EntityFrameworkCore.GlobalFilters;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.Abp.EntityFrameworkCore;

public class AdditionalProviderConfigurationTests
{
    [Theory]
    [InlineData(DatabaseProvider.Dm, false, false)]
    [InlineData(DatabaseProvider.Dm, false, true)]
    [InlineData(DatabaseProvider.Dm, true, false)]
    [InlineData(DatabaseProvider.Dm, true, true)]
    [InlineData(DatabaseProvider.GaussDB, false, false)]
    [InlineData(DatabaseProvider.GaussDB, false, true)]
    [InlineData(DatabaseProvider.GaussDB, true, false)]
    [InlineData(DatabaseProvider.GaussDB, true, true)]
    public void Configuration_Preserves_Connections_And_Allows_Overriding_Default_Split_Queries(
        DatabaseProvider provider, bool reuseConnection, bool overrideDefaults)
    {
        using var services = new ServiceCollection().AddLogging().BuildServiceProvider();
        var connectionString = provider == DatabaseProvider.Dm
            ? "Server=localhost;User Id=app;Password=unused"
            : "Host=localhost;Database=app;Username=app";
        using DbConnection connection = provider == DatabaseProvider.Dm
            ? new DmConnection(connectionString) : new GaussDBConnection(connectionString);
        var context = new AbpDbContextConfigurationContext(connectionString, services, "Default",
            reuseConnection ? connection : null);

        if (provider == DatabaseProvider.Dm)
        {
            context.UseDm(overrideDefaults ? builder => builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)
                .CommandTimeout(19).MigrationsAssembly("Application.Migrations") : null);
        }
        else
        {
            context.UseGaussDB(overrideDefaults ? builder => builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)
                .CommandTimeout(19).MigrationsAssembly("Application.Migrations") : null);
        }

        var extension = RelationalOptionsExtension.Extract(context.DbContextOptions.Options);
        Assert.Equal(overrideDefaults ? QuerySplittingBehavior.SingleQuery : QuerySplittingBehavior.SplitQuery,
            extension.QuerySplittingBehavior);
        Assert.Equal(overrideDefaults ? 19 : null, extension.CommandTimeout);
        Assert.Equal(overrideDefaults ? "Application.Migrations" : null, extension.MigrationsAssembly);
        if (reuseConnection)
        {
            Assert.Same(connection, extension.Connection);
            Assert.False(extension.IsConnectionOwned);
        }
        else
        {
            Assert.Equal(connectionString, extension.ConnectionString);
        }
    }

    [Fact]
    public void Model_Markers_Replace_And_Clear_Provider_Identity()
    {
        var builder = new ModelBuilder();
        Assert.False(AbpDmModelBuilderExtensions.IsUsingDm(builder));
        Assert.False(AbpGaussDBModelBuilderExtensions.IsUsingGaussDB(builder));
        builder.UseDm();
        Assert.True(AbpDmModelBuilderExtensions.IsUsingDm(builder));
        Assert.Equal(DatabaseProvider.Dm, builder.GetSharpAbpDatabaseProvider());
        builder.UseGaussDB();
        Assert.True(AbpGaussDBModelBuilderExtensions.IsUsingGaussDB(builder));
        Assert.False(AbpDmModelBuilderExtensions.IsUsingDm(builder));
        builder.ClearSharpAbpDatabaseProvider();
        Assert.False(AbpGaussDBModelBuilderExtensions.IsUsingGaussDB(builder));
    }

    [Fact]
    public async Task Connection_Checkers_Reject_Empty_And_Malformed_Settings()
    {
        var dm = new DmConnectionStringChecker(NullLogger<DmConnectionStringChecker>.Instance);
        var gauss = new GaussDBConnectionStringChecker(NullLogger<GaussDBConnectionStringChecker>.Instance);
        Assert.Throws<ArgumentNullException>(() => new DmConnectionStringChecker(null));
        Assert.Throws<ArgumentNullException>(() => new GaussDBConnectionStringChecker(null));
        foreach (var checker in new IConnectionStringChecker[] { dm, gauss })
        {
            await Assert.ThrowsAnyAsync<ArgumentException>(() => checker.CheckAsync(" "));
            var result = await checker.CheckAsync("Password='unterminated");
            Assert.False(result.Connected);
            Assert.False(result.DatabaseExists);
        }
    }

    [Fact]
    public async Task Connection_Checkers_Report_Refused_Loopback_Connections_Without_A_Database()
    {
        // Reserve a port without listening so no database service can receive the check.
        using var reservation = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        reservation.ExclusiveAddressUse = true;
        reservation.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        var port = ((IPEndPoint)reservation.LocalEndPoint).Port;
        var dm = new DmConnectionStringChecker(NullLogger<DmConnectionStringChecker>.Instance);
        var gauss = new GaussDBConnectionStringChecker(NullLogger<GaussDBConnectionStringChecker>.Instance);

        var dmResult = await dm.CheckAsync($"Server=127.0.0.1;Port={port};User Id=app;Password=unused");
        var gaussResult = await gauss.CheckAsync($"Host=127.0.0.1;Port={port};Database=app;Username=app;Pooling=false");

        Assert.False(dmResult.Connected);
        Assert.False(dmResult.DatabaseExists);
        Assert.False(gaussResult.Connected);
        Assert.False(gaussResult.DatabaseExists);
    }

    [Fact]
    public async Task Connection_Checkers_Report_Incomplete_Or_Invalid_Provider_Settings()
    {
        var dm = new DmConnectionStringChecker(NullLogger<DmConnectionStringChecker>.Instance);
        var gauss = new GaussDBConnectionStringChecker(NullLogger<GaussDBConnectionStringChecker>.Instance);

        Assert.False((await dm.CheckAsync("Server=localhost;Port=not-a-number")).Connected);
        Assert.False((await gauss.CheckAsync("Database=app;Username=app")).Connected);
    }

    [Theory]
    [InlineData("Port=2147483648")]
    [InlineData("Port=-1")]
    [InlineData("Max Pool Size=2147483648")]
    public async Task Dm_Checker_Rejects_Numeric_Settings_Outside_Their_Range(string invalidSetting)
    {
        var checker = new DmConnectionStringChecker(NullLogger<DmConnectionStringChecker>.Instance);
        var result = await checker.CheckAsync("Server=127.0.0.1;User Id=app;Password=unused;" + invalidSetting);
        Assert.False(result.Connected);
        Assert.False(result.DatabaseExists);
    }
}

public class DmModuleTests : AbpIntegratedTest<DmProviderTestModule>
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options) => options.UseAutofac();

    [Fact]
    public async Task Module_Applies_Defaults_And_Typed_Context_Override()
    {
        Assert.IsType<DmConnectionStringChecker>(GetRequiredService<IConnectionStringChecker>());
        Assert.Equal(SequentialGuidType.SequentialAsString,
            GetRequiredService<IOptions<AbpSequentialGuidGeneratorOptions>>().Value.DefaultSequentialGuidType);
        Assert.True(GetRequiredService<IOptions<AbpEfCoreGlobalFilterOptions>>().Value.UseDbFunction);
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var global = await GetRequiredService<IDbContextProvider<DmGlobalContext>>().GetDbContextAsync();
        var typed = await GetRequiredService<IDbContextProvider<DmTypedContext>>().GetDbContextAsync();
        Assert.IsType<DmConnection>(global.Database.GetDbConnection());
        Assert.Equal(QuerySplittingBehavior.SplitQuery, RelationalOptionsExtension.Extract(global.GetService<IDbContextOptions>()).QuerySplittingBehavior);
        Assert.Equal(QuerySplittingBehavior.SingleQuery, RelationalOptionsExtension.Extract(typed.GetService<IDbContextOptions>()).QuerySplittingBehavior);
        Assert.Equal(23, typed.Database.GetCommandTimeout());
        await unitOfWork.CompleteAsync();
    }
}

public class GaussDBModuleTests : AbpIntegratedTest<GaussDBProviderTestModule>
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options) => options.UseAutofac();

    [Fact]
    public async Task Module_Preserves_Explicit_Guid_Order_And_Typed_Context_Override()
    {
        Assert.IsType<GaussDBConnectionStringChecker>(GetRequiredService<IConnectionStringChecker>());
        Assert.Equal(SequentialGuidType.SequentialAsBinary,
            GetRequiredService<IOptions<AbpSequentialGuidGeneratorOptions>>().Value.DefaultSequentialGuidType);
        Assert.True(GetRequiredService<IOptions<AbpEfCoreGlobalFilterOptions>>().Value.UseDbFunction);
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var global = await GetRequiredService<IDbContextProvider<GaussDBGlobalContext>>().GetDbContextAsync();
        var typed = await GetRequiredService<IDbContextProvider<GaussDBTypedContext>>().GetDbContextAsync();
        Assert.IsType<GaussDBConnection>(global.Database.GetDbConnection());
        Assert.Equal(QuerySplittingBehavior.SplitQuery, RelationalOptionsExtension.Extract(global.GetService<IDbContextOptions>()).QuerySplittingBehavior);
        Assert.Equal(QuerySplittingBehavior.SingleQuery, RelationalOptionsExtension.Extract(typed.GetService<IDbContextOptions>()).QuerySplittingBehavior);
        Assert.Equal(23, typed.Database.GetCommandTimeout());
        await unitOfWork.CompleteAsync();
    }
}

[DependsOn(typeof(AbpEntityFrameworkCoreDmModule), typeof(AbpAutofacModule), typeof(AbpTestBaseModule))]
public class DmProviderTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<DmGlobalContext>();
        context.Services.AddAbpDbContext<DmTypedContext>();
        Configure<AbpDbConnectionOptions>(options => options.ConnectionStrings.Default = "Server=localhost;User Id=app;Password=unused");
        Configure<AbpDbContextOptions>(options =>
        {
            options.UseDm();
            options.UseDm<DmTypedContext>(builder => builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery).CommandTimeout(23));
        });
    }
}

[DependsOn(typeof(AbpEntityFrameworkCoreGaussDBModule), typeof(AbpAutofacModule), typeof(AbpTestBaseModule))]
public class GaussDBProviderTestModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
        => Configure<AbpSequentialGuidGeneratorOptions>(options => options.DefaultSequentialGuidType = SequentialGuidType.SequentialAsBinary);

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<GaussDBGlobalContext>();
        context.Services.AddAbpDbContext<GaussDBTypedContext>();
        Configure<AbpDbConnectionOptions>(options => options.ConnectionStrings.Default = "Host=localhost;Database=app;Username=app");
        Configure<AbpDbContextOptions>(options =>
        {
            options.UseGaussDB();
            options.UseGaussDB<GaussDBTypedContext>(builder => builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery).CommandTimeout(23));
        });
    }
}

public class DmGlobalContext(DbContextOptions<DmGlobalContext> options) : AbpDbContext<DmGlobalContext>(options);
public class DmTypedContext(DbContextOptions<DmTypedContext> options) : AbpDbContext<DmTypedContext>(options);
public class GaussDBGlobalContext(DbContextOptions<GaussDBGlobalContext> options) : AbpDbContext<GaussDBGlobalContext>(options);
public class GaussDBTypedContext(DbContextOptions<GaussDBTypedContext> options) : AbpDbContext<GaussDBTypedContext>(options);
