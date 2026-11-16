using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging.Abstractions;
using SharpAbp.Abp.Data;
using Xunit;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public class LargeTextColumnConventionTests
    {
        private const string MySqlProviderName = "Pomelo.EntityFrameworkCore.MySql";
        private const string DmProviderName = "DM.Microsoft.EntityFrameworkCore";
        private const string OracleProviderName = "Oracle.EntityFrameworkCore";
        private const string OceanBaseProviderName = "OceanBase.EntityFrameworkCore";

        [Fact]
        public void MySql_Row_Size_Overflow_Maps_Long_Columns_To_LongText()
        {
            using var dbContext = CreateDbContext<MySqlTag>(MySqlProviderName, new AbpLargeTextColumnOptions());

            // Two 8192 character columns exceed the 65535 byte row limit of MySQL.
            Assert.Equal("longtext", GetConfiguredColumnType(dbContext, nameof(TestEntity.Long8192)));
            Assert.Equal("longtext", GetConfiguredColumnType(dbContext, nameof(TestEntity.Huge)));
            // The row size rule converts every long column of the table.
            Assert.Equal("longtext", GetConfiguredColumnType(dbContext, nameof(TestEntity.Exactly2000)));
            // An explicitly configured column type is never changed.
            Assert.Equal("varchar(8192)", GetConfiguredColumnType(dbContext, nameof(TestEntity.Explicit)));
            // Indexed and concurrency token columns are skipped.
            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Indexed)));
            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Concurrency)));
            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Short)));
        }

        [Fact]
        public void Dm_Maps_Columns_Above_The_Inline_Limit_To_Lob()
        {
            using var dbContext = CreateDbContext<DmTag>(DmProviderName, new AbpLargeTextColumnOptions());

            Assert.Equal("CLOB", GetConfiguredColumnType(dbContext, nameof(TestEntity.Long8192)));
            Assert.Equal("CLOB", GetConfiguredColumnType(dbContext, nameof(TestEntity.Huge)));
            Assert.Equal("BLOB", GetConfiguredColumnType(dbContext, nameof(TestEntity.Binary)));

            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Exactly2000)));
            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Short)));
            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Indexed)));
            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Concurrency)));
        }

        [Fact]
        public void Providers_That_Widen_Long_Columns_Themselves_Are_Not_Changed()
        {
            using var dbContext = CreateDbContext<OracleTag>(OracleProviderName, new AbpLargeTextColumnOptions());

            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Long8192)));
            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Huge)));
            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Binary)));
        }

        [Fact]
        public void OceanBase_Maps_Columns_Above_The_Nvarchar2_Limit_To_Clob()
        {
            using var dbContext = CreateDbContext<OceanBaseTag>(OceanBaseProviderName, new AbpLargeTextColumnOptions());

            Assert.Equal("CLOB", GetConfiguredColumnType(dbContext, nameof(TestEntity.Huge)));
            Assert.Equal("BLOB", GetConfiguredColumnType(dbContext, nameof(TestEntity.Binary)));
            // 8192 characters still fit into NVARCHAR2 on OceanBase.
            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Long8192)));
        }

        [Fact]
        public void Disabled_Convention_Does_Nothing()
        {
            var options = new AbpLargeTextColumnOptions
            {
                Enabled = false
            };

            using var dbContext = CreateDbContext<DisabledTag>(MySqlProviderName, options);

            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Long8192)));
        }

        [Fact]
        public void Binary_Columns_Are_Ignored_When_IncludeBinary_Is_False()
        {
            var options = new AbpLargeTextColumnOptions
            {
                IncludeBinary = false
            };

            using var dbContext = CreateDbContext<NoBinaryTag>(DmProviderName, options);

            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Binary)));
            Assert.Equal("CLOB", GetConfiguredColumnType(dbContext, nameof(TestEntity.Long8192)));
        }

        [Fact]
        public void Provider_Settings_Can_Be_Overridden()
        {
            var options = new AbpLargeTextColumnOptions();
            options.Providers[DatabaseProvider.Dm] = new AbpLargeTextColumnProviderOptions
            {
                StringMaxLength = 10000,
                StringColumnType = "VARCHAR2(10000)",
                BinaryMaxLength = 10000,
                BinaryColumnType = "BLOB"
            };

            using var dbContext = CreateDbContext<OverrideTag>(DmProviderName, options);

            Assert.Equal("VARCHAR2(10000)", GetConfiguredColumnType(dbContext, nameof(TestEntity.Huge)));
            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Long8192)));
            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Binary)));
        }

        private static string GetConfiguredColumnType(DbContext dbContext, string propertyName)
        {
            var property = dbContext.Model
                .FindEntityType(typeof(TestEntity))
                .FindProperty(propertyName);

            return property
                .FindAnnotation(RelationalAnnotationNames.ColumnType)
                ?.Value as string;
        }

        private static ConventionDbContext<TTag> CreateDbContext<TTag>(
            string providerName,
            AbpLargeTextColumnOptions options)
            where TTag : class
        {
            var dbContextOptions = new DbContextOptionsBuilder<ConventionDbContext<TTag>>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            return new ConventionDbContext<TTag>(dbContextOptions, providerName, options);
        }

        private sealed class MySqlTag
        {
        }

        private sealed class DmTag
        {
        }

        private sealed class OracleTag
        {
        }

        private sealed class OceanBaseTag
        {
        }

        private sealed class DisabledTag
        {
        }

        private sealed class NoBinaryTag
        {
        }

        private sealed class OverrideTag
        {
        }
    }

    public class TestEntity
    {
        public int Id { get; set; }

        public string Short { get; set; }

        public string Exactly2000 { get; set; }

        public string Long8192 { get; set; }

        public string Huge { get; set; }

        public string Explicit { get; set; }

        public string Indexed { get; set; }

        public string Concurrency { get; set; }

        public byte[] Binary { get; set; }
    }

    public class ConventionDbContext<TTag> : DbContext
        where TTag : class
    {
        private readonly string _providerName;
        private readonly AbpLargeTextColumnOptions _largeTextColumnOptions;

        public ConventionDbContext(
            DbContextOptions<ConventionDbContext<TTag>> options,
            string providerName,
            AbpLargeTextColumnOptions largeTextColumnOptions)
            : base(options)
        {
            _providerName = providerName;
            _largeTextColumnOptions = largeTextColumnOptions;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>(builder =>
            {
                builder.ToTable("TestEntities");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Short).HasMaxLength(256);
                builder.Property(x => x.Exactly2000).HasMaxLength(2000);
                builder.Property(x => x.Long8192).HasMaxLength(8192);
                builder.Property(x => x.Huge).HasMaxLength(33000);
                builder.Property(x => x.Explicit).HasMaxLength(8192).HasColumnType("varchar(8192)");
                builder.Property(x => x.Indexed).HasMaxLength(8192);
                builder.HasIndex(x => x.Indexed);
                builder.Property(x => x.Concurrency).HasMaxLength(8192).IsConcurrencyToken();
                builder.Property(x => x.Binary).HasMaxLength(3000);
            });
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new LargeTextColumnConvention(
                _providerName,
                Microsoft.Extensions.Options.Options.Create(_largeTextColumnOptions),
                new EfCoreDatabaseProviderAccessor(NullLogger<EfCoreDatabaseProviderAccessor>.Instance),
                NullLogger<LargeTextColumnConvention>.Instance));
        }
    }
}
