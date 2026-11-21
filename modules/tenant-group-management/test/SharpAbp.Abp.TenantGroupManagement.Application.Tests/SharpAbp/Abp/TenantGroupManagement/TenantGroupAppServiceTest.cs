using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SharpAbp.Abp.TenantGroupManagement.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.TenantManagement;
using Xunit;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupAppServiceTest : TenantGroupManagementTestBase
    {
        private readonly ITenantAppService _tenantAppService;
        private readonly ITenantGroupAppService _tenantGroupAppService;
        public TenantGroupAppServiceTest()
        {
            _tenantAppService = GetRequiredService<ITenantAppService>();
            _tenantGroupAppService = new TenantGroupController(GetRequiredService<ITenantGroupAppService>());
        }

        [Fact]
        public async Task AddTenant_Rejects_Membership_In_Another_Group()
        {
            var first = await _tenantGroupAppService.CreateAsync(new CreateTenantGroupDto { Name = "first", IsActive = true });
            var second = await _tenantGroupAppService.CreateAsync(new CreateTenantGroupDto { Name = "second", IsActive = true });
            var input = new AddTenantDto { TenantId = Guid.NewGuid() };

            await _tenantGroupAppService.AddTenantAsync(first.Id, input);

            await Assert.ThrowsAsync<UserFriendlyException>(() => _tenantGroupAppService.AddTenantAsync(second.Id, input));
            Assert.Single((await _tenantGroupAppService.GetAsync(first.Id)).Tenants);
            Assert.Empty((await _tenantGroupAppService.GetAsync(second.Id)).Tenants);
        }

        [Fact]
        public async Task Relational_Schema_Rejects_The_Same_Tenant_In_Two_Groups()
        {
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var dbContext = new MembershipDbContext(
                new DbContextOptionsBuilder<MembershipDbContext>().UseSqlite(connection).Options);
            await dbContext.Database.EnsureCreatedAsync();

            var tenantId = Guid.NewGuid();
            var first = new TenantGroup(Guid.NewGuid(), "first", "FIRST", true);
            var second = new TenantGroup(Guid.NewGuid(), "second", "SECOND", true);
            first.AddTenant(new TenantGroupTenant(Guid.NewGuid(), first.Id, tenantId));
            second.AddTenant(new TenantGroupTenant(Guid.NewGuid(), second.Id, tenantId));
            dbContext.AddRange(first, second);

            var error = await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());
            Assert.Equal(2067, Assert.IsType<SqliteException>(error.InnerException).SqliteExtendedErrorCode);
        }

        private sealed class MembershipDbContext(DbContextOptions<MembershipDbContext> options) : DbContext(options)
        {
            protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ConfigureTenantGroupManagement();
        }

        [Fact]
        public async Task Curd_Test_Async()
        {

            var t1 = await _tenantAppService.CreateAsync(new TenantCreateDto()
            {
                Name = "tenant1",
                AdminEmailAddress = "tenant1@ABP.IO",
                AdminPassword = "1q2w3E*123",
            });

            var t2 = await _tenantAppService.CreateAsync(new TenantCreateDto()
            {
                Name = "tenant2",
                AdminEmailAddress = "tenant2@ABP.IO",
                AdminPassword = "1q2w3E*123",
            });


            var g1 = await _tenantGroupAppService.CreateAsync(new CreateTenantGroupDto()
            {
                Name = "group1",
                IsActive = true
            });

            await _tenantGroupAppService.UpdateDefaultConnectionStringAsync(g1.Id, "123456");
            var availableTenants = await _tenantGroupAppService.GetAvialableTenantsAsync();

            Assert.Equal(2, availableTenants.Count);
            await _tenantGroupAppService.AddTenantAsync(g1.Id, new AddTenantDto()
            {
                TenantId = t1.Id
            });


            availableTenants = await _tenantGroupAppService.GetAvialableTenantsAsync();
            Assert.Single(availableTenants);


            await _tenantGroupAppService.AddTenantAsync(g1.Id, new AddTenantDto()
            {
                TenantId = t2.Id
            });

            var g1_1 = await _tenantGroupAppService.GetAsync(g1.Id);
            Assert.Equal(2, g1_1.Tenants.Count);

            var g1_c1 = await _tenantGroupAppService.GetDefaultConnectionStringAsync(g1.Id);
            Assert.Equal("123456", g1_c1);

            await _tenantGroupAppService.DeleteAsync(g1.Id);

            await Assert.ThrowsAsync<EntityNotFoundException<TenantGroup>>(() =>
            {
                return _tenantGroupAppService.GetAsync(g1.Id);
            });


        }

    }
}
