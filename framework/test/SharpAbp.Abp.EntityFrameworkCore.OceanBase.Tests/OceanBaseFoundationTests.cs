using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using OceanBase.EntityFrameworkCore;
using OceanBaseConnector;
using SharpAbp.Abp.Data;
using SharpAbp.Abp.EntityFrameworkCore.ConnectionStrings;
using SharpAbp.Abp.EntityFrameworkCore.OceanBase;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Volo.Abp.Uow;
using Xunit;
using EfMode = OceanBase.EntityFrameworkCore.OceanBaseCompatibilityMode;

namespace SharpAbp.Abp.EntityFrameworkCore;

public class OceanBaseConfigurationTests
{
    private const string ConnectionString = "Server=localhost;Port=2881;User ID=app;Database=appdb";

    [Theory]
    [InlineData(EfMode.MySql)]
    [InlineData(EfMode.Oracle)]
    public void Context_Configuration_Preserves_Mode_And_Callback(EfMode mode)
    {
        using var services = new ServiceCollection().AddLogging().BuildServiceProvider();
        var context = new AbpDbContextConfigurationContext(ConnectionString, services, "Default", null);

        context.UseOceanBase(mode, options => options.MigrationsAssembly("Application.Migrations")
            .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));

        var extension = RelationalOptionsExtension.Extract(context.DbContextOptions.Options);
        Assert.NotNull(extension);
        Assert.Equal(ConnectionString, extension.ConnectionString);
        Assert.Equal("Application.Migrations", extension.MigrationsAssembly);
        Assert.Equal(QuerySplittingBehavior.SingleQuery, extension.QuerySplittingBehavior);
        using var dbContext = new DbContext(context.DbContextOptions.Options);
        Assert.True(dbContext.Database.IsOceanBase());
        var native = new OceanBaseConnectionStringBuilder(dbContext.Database.GetDbConnection().ConnectionString);
        Assert.Equal(mode.ToString(), native.ExpectedMode.ToString());
    }

    [Fact]
    public void Existing_Connection_Takes_Precedence_And_Remains_Caller_Owned()
    {
        using var services = new ServiceCollection().AddLogging().BuildServiceProvider();
        using var connection = new OceanBaseConnection(ConnectionString);
        var context = new AbpDbContextConfigurationContext("unused", services, "Default", connection);
        context.UseOceanBase(EfMode.MySql);

        using (var dbContext = new DbContext(context.DbContextOptions.Options))
        {
            Assert.Same(connection, dbContext.Database.GetDbConnection());
        }

        Assert.Equal(ConnectionState.Closed, connection.State);
        Assert.Equal("appdb", connection.Database);
        Assert.Equal(ConnectionString, connection.ConnectionString);
    }

    [Fact]
    public void Invalid_Mode_Rejects_At_Configuration_Time()
    {
        var options = new AbpDbContextOptions();
        Assert.Throws<ArgumentOutOfRangeException>(() => options.UseOceanBase((EfMode)123));
        Assert.Throws<ArgumentOutOfRangeException>(() => options.UseOceanBase<OracleTestDbContext>((EfMode)123));
    }

    [Fact]
    public void Model_Marker_Identifies_OceanBase_Without_Oracle_Or_MySql_Aliasing()
    {
        var builder = new ModelBuilder();
        builder.UseOceanBase();
        Assert.True(builder.IsUsingOceanBase());
        Assert.Equal(DatabaseProvider.OceanBase, builder.GetSharpAbpDatabaseProvider());
        builder.ClearSharpAbpDatabaseProvider();
        Assert.False(builder.IsUsingOceanBase());
    }

    [Fact]
    public async Task Checker_Returns_Failure_For_Malformed_Connection_Without_Logging_Secrets()
    {
        var logger = new CapturingLogger();
        var checker = new OceanBaseConnectionStringChecker(logger);
        var result = await checker.CheckAsync("Password='not-terminated");
        Assert.False(result.Connected);
        Assert.False(result.DatabaseExists);
        Assert.DoesNotContain("not-terminated", logger.Message);
        Assert.Null(logger.Exception);
        await Assert.ThrowsAsync<ArgumentException>(() => checker.CheckAsync(" "));
    }

    private sealed class CapturingLogger : ILogger<OceanBaseConnectionStringChecker>
    {
        public string Message { get; private set; } = string.Empty;
        public Exception Exception { get; private set; }
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            Message = formatter(state, exception);
            Exception = exception;
        }
    }
}

public class OceanBaseModuleTests : AbpIntegratedTest<OceanBaseFoundationTestModule>
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        => options.UseAutofac();

    [Fact]
    public void Module_Registers_OceanBase_Connection_Checker()
        => Assert.IsType<OceanBaseConnectionStringChecker>(GetRequiredService<IConnectionStringChecker>());

    [Fact]
    public async Task Global_And_Typed_Configuration_Keep_Models_Isolated_By_Mode()
    {
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);
        var mysql = await GetRequiredService<IDbContextProvider<MySqlTestDbContext>>().GetDbContextAsync();
        var oracle = await GetRequiredService<IDbContextProvider<OracleTestDbContext>>().GetDbContextAsync();

        Assert.Equal("MySql", new OceanBaseConnectionStringBuilder(mysql.Database.GetDbConnection().ConnectionString).ExpectedMode.ToString());
        Assert.Equal("Oracle", new OceanBaseConnectionStringBuilder(oracle.Database.GetDbConnection().ConnectionString).ExpectedMode.ToString());
        AssertColumns(mysql, "LONGTEXT", "LONGTEXT", "LONGBLOB", "VARCHAR(120)");
        AssertColumns(oracle, "NCLOB", "CLOB", "BLOB", "VARCHAR2(120 CHAR)");
        AssertColumns(mysql, "LONGTEXT", "LONGTEXT", "LONGBLOB", "VARCHAR(120)");
        await unitOfWork.CompleteAsync();
    }

    private static void AssertColumns(DbContext context, string textType, string ansiType, string binaryType, string explicitType)
    {
        var entity = context.Model.FindEntityType(typeof(LargeValue));
        Assert.Equal(textType, entity.FindProperty(nameof(LargeValue.Text)).GetColumnType());
        Assert.Equal(ansiType, entity.FindProperty(nameof(LargeValue.AnsiText)).GetColumnType());
        Assert.Equal(binaryType, entity.FindProperty(nameof(LargeValue.Binary)).GetColumnType());
        Assert.Equal(explicitType, entity.FindProperty(nameof(LargeValue.Explicit)).GetColumnType());
    }
}

[DependsOn(typeof(AbpEntityFrameworkCoreOceanBaseModule), typeof(AbpAutofacModule), typeof(AbpTestBaseModule))]
public class OceanBaseFoundationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<MySqlTestDbContext>();
        context.Services.AddAbpDbContext<OracleTestDbContext>();
        Configure<AbpDbConnectionOptions>(options => options.ConnectionStrings.Default =
            "Server=localhost;Port=2881;User ID=app;Database=appdb");
        Configure<AbpDbContextOptions>(options =>
        {
            options.UseOceanBase(EfMode.MySql);
            options.UseOceanBase<OracleTestDbContext>(EfMode.Oracle);
        });
    }
}

public class MySqlTestDbContext(DbContextOptions<MySqlTestDbContext> options) : AbpDbContext<MySqlTestDbContext>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        ConfigureValues(builder);
    }

    internal static void ConfigureValues(ModelBuilder builder)
    {
        builder.Entity<LargeValue>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.AnsiText).IsUnicode(false);
            entity.Property(x => x.Explicit).HasColumnType("VARCHAR(120)");
        });
    }
}

public class OracleTestDbContext(DbContextOptions<OracleTestDbContext> options) : AbpDbContext<OracleTestDbContext>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        MySqlTestDbContext.ConfigureValues(builder);
        builder.Entity<LargeValue>().Property(x => x.Explicit).HasColumnType("VARCHAR2(120 CHAR)");
    }
}

public class LargeValue
{
    public int Id { get; set; }
    [LargeTextColumn] public string Text { get; set; }
    [LargeTextColumn] public string AnsiText { get; set; }
    [LargeTextColumn] public byte[] Binary { get; set; }
    [LargeTextColumn] public string Explicit { get; set; }
}
