using System;
using Microsoft.EntityFrameworkCore;
using SharpAbp.Abp.Data;
using Xunit;
using EfMode = OceanBase.EntityFrameworkCore.OceanBaseCompatibilityMode;

namespace SharpAbp.Abp.EntityFrameworkCore;

public class OceanBaseDesignTimeTests
{
    [Fact]
    public void Same_Context_Type_Uses_Separate_Models_For_Each_Mode_Without_Abp_Injection()
    {
        using var mysql = CreateContext(EfMode.MySql);
        using var oracle = CreateContext(EfMode.Oracle);
        Assert.NotSame(mysql.Model, oracle.Model);
        AssertTypes(mysql, "LONGTEXT", "LONGBLOB", "VARCHAR(8192)", "VARCHAR(128)");
        AssertTypes(oracle, "NCLOB", "BLOB", "NCLOB", "NVARCHAR2(128)");
        using var mysqlAgain = CreateContext(EfMode.MySql);
        Assert.Same(mysql.Model, mysqlAgain.Model);
        AssertTypes(mysqlAgain, "LONGTEXT", "LONGBLOB", "VARCHAR(8192)", "VARCHAR(128)");
    }

    [Theory]
    [InlineData(EfMode.MySql)]
    [InlineData(EfMode.Oracle)]
    public void Invalid_Convention_Mode_Rejects_Before_Model_Creation(EfMode mode)
    {
        var options = new DbContextOptionsBuilder<InvalidModeDbContext>()
            .UseOceanBase("Server=localhost;User ID=app", mode).Options;
        using var context = new InvalidModeDbContext(options);
        Assert.Throws<ArgumentOutOfRangeException>(() => context.Model);
    }

    private static DesignTimeDbContext CreateContext(EfMode mode)
        => new(new DbContextOptionsBuilder<DesignTimeDbContext>()
            .UseOceanBase("Server=localhost;Port=2881;User ID=app;Database=appdb", mode).Options, mode);

    private static void AssertTypes(DbContext context, string lob, string binary, string ordinary, string bounded)
    {
        var entity = context.Model.FindEntityType(typeof(DesignTimeValue));
        Assert.Equal(lob, entity.FindProperty(nameof(DesignTimeValue.Marked)).GetColumnType());
        Assert.Equal(binary, entity.FindProperty(nameof(DesignTimeValue.Binary)).GetColumnType());
        Assert.Equal(ordinary, entity.FindProperty(nameof(DesignTimeValue.Ordinary)).GetColumnType());
        Assert.Equal(bounded, entity.FindProperty(nameof(DesignTimeValue.Indexed)).GetColumnType());
        Assert.Equal(bounded, entity.FindProperty(nameof(DesignTimeValue.Converted)).GetColumnType());
    }

    private sealed class DesignTimeDbContext(DbContextOptions<DesignTimeDbContext> options, EfMode mode) : DbContext(options)
    {
        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
            => builder.AddSharpAbpOceanBaseLargeTextColumnConvention(this, mode);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DesignTimeValue>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();
                entity.Property(x => x.Ordinary).HasMaxLength(8192);
                entity.Property(x => x.Indexed).HasMaxLength(128);
                entity.HasIndex(x => x.Indexed);
                entity.Property(x => x.Converted).HasMaxLength(128).HasConversion(value => value, value => value);
            });
        }
    }

    private sealed class InvalidModeDbContext(DbContextOptions<InvalidModeDbContext> options) : DbContext(options)
    {
        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
            => builder.AddSharpAbpOceanBaseLargeTextColumnConvention(this, (EfMode)123);
    }

    private sealed class DesignTimeValue
    {
        public int Id { get; set; }
        [LargeTextColumn] public string Marked { get; set; }
        [LargeTextColumn] public byte[] Binary { get; set; }
        public string Ordinary { get; set; }
        [LargeTextColumn] public string Indexed { get; set; }
        [LargeTextColumn] public string Converted { get; set; }
    }
}
