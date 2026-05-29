using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.FileStoring.MapTenancy;
using SharpAbp.Abp.MapTenancy;
using Volo.Abp;
using Volo.Abp.MultiTenancy;
using Xunit;

#nullable enable

namespace SharpAbp.Abp.FileStoring.MapTenancy
{
    public class MapTenancyFilePathContextResolveContributorTest : AbpFileStoringAllTestBase
    {
        [Fact]
        public async Task ResolveAsync_UsesMapTenancyCode_AsTenantCode()
        {
            var tenantId = Guid.NewGuid();
            var contributor = new MapTenancyFilePathContextResolveContributor();
            var serviceProvider = BuildServiceProvider(
                tenantId,
                new MapTenantCodeInfo
                {
                    TenantId = tenantId,
                    TenantName = "Tenant One",
                    Code = "T001",
                    MapCode = "M001"
                },
                new AbpFileStoringMapTenancyOptions());
            var context = new FilePathContextResolveContext(serviceProvider);

            await contributor.ResolveAsync(context);

            Assert.True(context.Handled);
            Assert.Equal("T001", context.FilePathContext!.TenantCode);
            Assert.Equal("M001", context.FilePathContext.Extra["MapCode"]);
            Assert.Equal("MapTenancy", context.FilePathContext.Extra["Source"]);
        }

        [Fact]
        public async Task ResolveAsync_CanUseMapCode_AsTenantCode()
        {
            var tenantId = Guid.NewGuid();
            var contributor = new MapTenancyFilePathContextResolveContributor();
            var options = new AbpFileStoringMapTenancyOptions
            {
                TenantCodeSource = FilePathTenantCodeSource.MapCode
            };
            var serviceProvider = BuildServiceProvider(
                tenantId,
                new MapTenantCodeInfo
                {
                    TenantId = tenantId,
                    Code = "T001",
                    MapCode = "M001"
                },
                options);
            var context = new FilePathContextResolveContext(serviceProvider);

            await contributor.ResolveAsync(context);

            Assert.True(context.Handled);
            Assert.Equal("M001", context.FilePathContext!.TenantCode);
        }

        [Fact]
        public async Task ResolveAsync_Ignores_WhenMapTenantMissingByDefault()
        {
            var contributor = new MapTenancyFilePathContextResolveContributor();
            var serviceProvider = BuildServiceProvider(
                Guid.NewGuid(),
                null,
                new AbpFileStoringMapTenancyOptions());
            var context = new FilePathContextResolveContext(serviceProvider);

            await contributor.ResolveAsync(context);

            Assert.False(context.Handled);
            Assert.Null(context.FilePathContext);
        }

        [Fact]
        public async Task ResolveAsync_Throws_WhenMapTenantMissing_AndBehaviorIsThrow()
        {
            var contributor = new MapTenancyFilePathContextResolveContributor();
            var serviceProvider = BuildServiceProvider(
                Guid.NewGuid(),
                null,
                new AbpFileStoringMapTenancyOptions
                {
                    MissingMapTenantBehavior = MissingMapTenantBehavior.Throw
                });
            var context = new FilePathContextResolveContext(serviceProvider);

            await Assert.ThrowsAsync<AbpException>(() => contributor.ResolveAsync(context));
        }

        [Fact]
        public async Task ResolveAsync_Ignores_WhenCurrentTenantIsHost()
        {
            var contributor = new MapTenancyFilePathContextResolveContributor();
            var services = new ServiceCollection();
            services.AddSingleton<ICurrentTenant>(new TestCurrentTenant(null));
            services.AddSingleton<IMapTenantCodeProvider>(
                new ThrowingMapTenantCodeProvider());
            services.AddSingleton<IOptions<AbpFileStoringMapTenancyOptions>>(
                Options.Create(new AbpFileStoringMapTenancyOptions()));
            var context = new FilePathContextResolveContext(services.BuildServiceProvider());

            await contributor.ResolveAsync(context);

            Assert.False(context.Handled);
            Assert.Null(context.FilePathContext);
        }

        [Fact]
        public async Task ResolveAsync_Ignores_WhenSelectedTenantCodeIsEmpty()
        {
            var tenantId = Guid.NewGuid();
            var contributor = new MapTenancyFilePathContextResolveContributor();
            var serviceProvider = BuildServiceProvider(
                tenantId,
                new MapTenantCodeInfo
                {
                    TenantId = tenantId,
                    Code = "T001",
                    MapCode = ""
                },
                new AbpFileStoringMapTenancyOptions
                {
                    TenantCodeSource = FilePathTenantCodeSource.MapCode
                });
            var context = new FilePathContextResolveContext(serviceProvider);

            await contributor.ResolveAsync(context);

            Assert.False(context.Handled);
            Assert.Null(context.FilePathContext);
        }

        private static IServiceProvider BuildServiceProvider(
            Guid tenantId,
            MapTenantCodeInfo? codeInfo,
            AbpFileStoringMapTenancyOptions options)
        {
            var services = new ServiceCollection();
            services.AddSingleton<ICurrentTenant>(new TestCurrentTenant(tenantId));
            services.AddSingleton<IMapTenantCodeProvider>(new TestMapTenantCodeProvider(codeInfo));
            services.AddSingleton<IOptions<AbpFileStoringMapTenancyOptions>>(Options.Create(options));
            return services.BuildServiceProvider();
        }

        private class TestMapTenantCodeProvider : IMapTenantCodeProvider
        {
            private readonly MapTenantCodeInfo? _codeInfo;

            public TestMapTenantCodeProvider(MapTenantCodeInfo? codeInfo)
            {
                _codeInfo = codeInfo;
            }

            public Task<MapTenantCodeInfo?> FindByTenantIdAsync(Guid tenantId)
            {
                return Task.FromResult(_codeInfo);
            }
        }

        private class TestCurrentTenant : ICurrentTenant
        {
            public TestCurrentTenant(Guid? id)
            {
                Id = id;
            }

            public bool IsAvailable => true;

            public Guid? Id { get; }

            public string? Name => "Tenant One";

            public IDisposable Change(Guid? id, string? name = null)
            {
                return NullDisposable.Instance;
            }
        }

        private class ThrowingMapTenantCodeProvider : IMapTenantCodeProvider
        {
            public Task<MapTenantCodeInfo?> FindByTenantIdAsync(Guid tenantId)
            {
                throw new InvalidOperationException("Provider should not be called for host tenant.");
            }
        }
    }
}
