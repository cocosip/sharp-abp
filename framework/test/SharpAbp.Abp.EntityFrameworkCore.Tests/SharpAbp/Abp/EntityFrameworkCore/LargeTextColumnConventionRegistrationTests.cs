using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Data;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public class LargeTextColumnConventionRegistrationTests : AbpIntegratedTest<LargeTextColumnConventionTestModule>
    {
        private readonly IDbContextProvider<LargeTextColumnConventionTestDbContext> _dbContextProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public LargeTextColumnConventionRegistrationTests()
        {
            _dbContextProvider = GetRequiredService<IDbContextProvider<LargeTextColumnConventionTestDbContext>>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
        }

        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }

        [Fact]
        public async Task Convention_Is_Applied_To_AbpDbContext_Models_Automatically()
        {
            using (var unitOfWork = _unitOfWorkManager.Begin(requiresNew: true))
            {
                var dbContext = await _dbContextProvider.GetDbContextAsync();

                var options = GetRequiredService<Microsoft.Extensions.Options.IOptions<AbpLargeTextColumnOptions>>().Value;
                Assert.True(options.Providers.ContainsKey(DatabaseProvider.InMemory));

                var property = dbContext.Model
                    .FindEntityType(typeof(LargeTextColumnTestEntity))
                    .FindProperty(nameof(LargeTextColumnTestEntity.Value));

                Assert.Equal("TEST_LOB", property.FindAnnotation(RelationalAnnotationNames.ColumnType)?.Value as string);

                await unitOfWork.CompleteAsync();
            }
        }
    }

    [DependsOn(typeof(AbpEntityFrameworkCoreTestModule))]
    public class LargeTextColumnConventionTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLargeTextColumnOptions>(options =>
            {
                // The InMemory provider is used to verify that the convention is registered for the
                // models of AbpDbContexts without relying on a real database.
                options.Providers[DatabaseProvider.InMemory] = new AbpLargeTextColumnProviderOptions
                {
                    StringMaxLength = 1000,
                    StringColumnType = "TEST_LOB"
                };
            });

            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(configurationContext =>
                    configurationContext.DbContextOptions.UseInMemoryDatabase("LargeTextColumnConventionTest"));
            });

            context.Services.AddAbpDbContext<LargeTextColumnConventionTestDbContext>(options =>
            {
                options.AddDefaultRepositories(includeAllEntities: true);
            });
        }
    }

    public class LargeTextColumnConventionTestDbContext : AbpDbContext<LargeTextColumnConventionTestDbContext>
    {
        public DbSet<LargeTextColumnTestEntity> Entities { get; set; }

        public LargeTextColumnConventionTestDbContext(
            DbContextOptions<LargeTextColumnConventionTestDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<LargeTextColumnTestEntity>(b =>
            {
                b.ToTable("LargeTextColumnTestEntities");
                b.ConfigureByConvention();
                b.Property(x => x.Value).HasMaxLength(2000);
            });
        }
    }

    public class LargeTextColumnTestEntity : Entity<Guid>
    {
        public string Value { get; set; }
    }
}
