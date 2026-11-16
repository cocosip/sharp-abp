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
        private const string DmProviderName = "DM.EntityFrameworkCore";
        private const string OracleProviderName = "Oracle.EntityFrameworkCore";
        private const string SqlServerProviderName = "Microsoft.EntityFrameworkCore.SqlServer";
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

        [Fact]
        public void Convention_Can_Be_Registered_Without_Abp_Service_Provider()
        {
            // A DbContext created without ABP property injection (for example by a hand written
            // IDesignTimeDbContextFactory) registers the convention from ConfigureConventions.
            using var dbContext = new DesignTimeDbContext();

            var property = dbContext.Model
                .FindEntityType(typeof(TestEntity))
                .FindProperty(nameof(TestEntity.Long8192));

            // SQLite is not configured in the default options, so nothing is converted, but the
            // model has to build with the default options.
            Assert.Null(property.FindAnnotation(RelationalAnnotationNames.ColumnType)?.Value);
        }

        private sealed class DesignTimeDbContext : DbContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlite("Data Source=:memory:");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<TestEntity>(builder =>
                {
                    builder.HasKey(x => x.Id);
                    builder.Property(x => x.Long8192).HasMaxLength(8192);
                });
            }

            protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
            {
                configurationBuilder.AddSharpAbpLargeTextColumnConvention(this);
            }
        }

        [Fact]
        public void Attribute_Maps_Unbounded_Column_To_Oracle_Lob()
        {
            using var dbContext = CreateDbContext<OracleAttributeTag>(OracleProviderName, new AbpLargeTextColumnOptions());

            Assert.Equal("NCLOB", GetConfiguredColumnType(dbContext, nameof(TestEntity.Marked)));
            Assert.Equal("CLOB", GetConfiguredColumnType(dbContext, nameof(TestEntity.MarkedAnsi)));
            Assert.Equal("BLOB", GetConfiguredColumnType(dbContext, nameof(TestEntity.MarkedBinary)));
            // Indexed columns and explicit column types are never changed.
            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.MarkedIndexed)));
            Assert.Equal("varchar(8192)", GetConfiguredColumnType(dbContext, nameof(TestEntity.MarkedExplicit)));
        }

        [Fact]
        public void Attribute_Maps_Unbounded_Column_To_SqlServer_Lob()
        {
            using var dbContext = CreateDbContext<SqlServerAttributeTag>(SqlServerProviderName, new AbpLargeTextColumnOptions());

            Assert.Equal("nvarchar(max)", GetConfiguredColumnType(dbContext, nameof(TestEntity.Marked)));
            Assert.Equal("varchar(max)", GetConfiguredColumnType(dbContext, nameof(TestEntity.MarkedAnsi)));
            Assert.Equal("varbinary(max)", GetConfiguredColumnType(dbContext, nameof(TestEntity.MarkedBinary)));
        }

        [Fact]
        public void Attribute_Maps_Unbounded_Column_To_Dm_Lob()
        {
            using var dbContext = CreateDbContext<DmAttributeTag>(DmProviderName, new AbpLargeTextColumnOptions());

            Assert.Equal("CLOB", GetConfiguredColumnType(dbContext, nameof(TestEntity.Marked)));
            Assert.Equal("BLOB", GetConfiguredColumnType(dbContext, nameof(TestEntity.MarkedBinary)));
        }

        [Fact]
        public void Attribute_Is_Ignored_For_A_Provider_Without_Column_Types()
        {
            using var dbContext = CreateDbContext<InMemoryAttributeTag>(
                "Microsoft.EntityFrameworkCore.InMemory",
                new AbpLargeTextColumnOptions());

            Assert.Null(GetConfiguredColumnType(dbContext, nameof(TestEntity.Marked)));
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

        private sealed class OracleAttributeTag
        {
        }

        private sealed class SqlServerAttributeTag
        {
        }

        private sealed class DmAttributeTag
        {
        }

        private sealed class InMemoryAttributeTag
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

        [LargeTextColumn]
        public string Marked { get; set; }

        [LargeTextColumn]
        public string MarkedAnsi { get; set; }

        [LargeTextColumn]
        public byte[] MarkedBinary { get; set; }

        [LargeTextColumn]
        public string MarkedIndexed { get; set; }

        [LargeTextColumn]
        public string MarkedExplicit { get; set; }
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
                builder.Property(x => x.Marked);
                builder.Property(x => x.MarkedAnsi).IsUnicode(false);
                builder.Property(x => x.MarkedBinary);
                builder.Property(x => x.MarkedIndexed);
                builder.HasIndex(x => x.MarkedIndexed);
                builder.Property(x => x.MarkedExplicit).HasMaxLength(8192).HasColumnType("varchar(8192)");
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
