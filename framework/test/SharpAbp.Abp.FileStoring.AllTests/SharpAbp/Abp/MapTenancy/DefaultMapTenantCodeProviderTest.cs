using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.MapTenancy;
using Xunit;

namespace SharpAbp.Abp.MapTenancy
{
    public class DefaultMapTenantCodeProviderTest
    {
        [Fact]
        public async Task FindByTenantIdAsync_Returns_CodeInfo()
        {
            var tenantId = Guid.NewGuid();
            var options = new AbpMapTenancyOptions();
            options.Mappers.Configure("T001", configuration =>
            {
                configuration.TenantId = tenantId;
                configuration.TenantName = "Tenant One";
                configuration.Code = "T001";
                configuration.MapCode = "M001";
            });

            var provider = new DefaultMapTenantCodeProvider(Options.Create(options));

            var result = await provider.FindByTenantIdAsync(tenantId);

            Assert.NotNull(result);
            Assert.Equal(tenantId, result!.TenantId);
            Assert.Equal("Tenant One", result.TenantName);
            Assert.Equal("T001", result.Code);
            Assert.Equal("M001", result.MapCode);
        }

        [Fact]
        public async Task FindByTenantIdAsync_ReturnsNull_WhenTenantIsMissing()
        {
            var provider = new DefaultMapTenantCodeProvider(Options.Create(new AbpMapTenancyOptions()));

            var result = await provider.FindByTenantIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }
    }
}
