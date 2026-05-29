using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Xunit;

#nullable enable

namespace SharpAbp.Abp.FileStoring
{
    public class FilePathContextResolverTest : AbpFileStoringAllTestBase
    {
        [Fact]
        public async Task ResolveAsync_Stops_WhenContributorHandled()
        {
            var options = new AbpFilePathContextResolveOptions();
            var first = new TestFilePathContextResolveContributor("first", context =>
            {
                context.FilePathContext = new FilePathContext { TenantCode = "T001" };
                context.Handled = true;
            });
            var second = new TestFilePathContextResolveContributor("second", _ =>
            {
                throw new InvalidOperationException("Second contributor should not run.");
            });
            options.Contributors.Add(first);
            options.Contributors.Add(second);

            var resolver = new DefaultFilePathContextResolver(
                GetRequiredService<IServiceProvider>(),
                Options.Create(options));

            var result = await resolver.ResolveAsync();

            Assert.Equal("T001", result!.TenantCode);
            Assert.Equal(1, first.CallCount);
            Assert.Equal(0, second.CallCount);
        }

        [Fact]
        public async Task FileContainer_UsesResolvedContext_WhenCurrentContextIsNull()
        {
            var tenantId = Guid.NewGuid();
            var currentTenant = GetRequiredService<ICurrentTenant>();
            var accessor = GetRequiredService<IFilePathContextAccessor>();
            var provider = new CapturingFileProvider();

            var container = new FileContainer(
                "default",
                new FileContainerConfiguration
                {
                    Provider = provider.Provider,
                    IsMultiTenant = true
                },
                provider,
                currentTenant,
                GetRequiredService<ICancellationTokenProvider>(),
                GetRequiredService<IFileNormalizeNamingService>(),
                GetRequiredService<IServiceProvider>(),
                accessor,
                new TestFilePathContextResolver(new FilePathContext { TenantCode = "CODE-001" }));

            using (currentTenant.Change(tenantId))
            {
                await container.SaveAsync("a.txt", new MemoryStream([1]), "txt");
            }

            Assert.Equal("CODE-001", provider.ContextSeenOnSave!.TenantCode);
            Assert.Null(accessor.Current);
        }

        [Fact]
        public async Task FileContainer_DoesNotResolve_WhenCurrentContextAlreadyExists()
        {
            var currentTenant = GetRequiredService<ICurrentTenant>();
            var accessor = GetRequiredService<IFilePathContextAccessor>();
            var provider = new CapturingFileProvider();
            var resolver = new TestFilePathContextResolver(new FilePathContext { TenantCode = "AUTO" });

            var container = new FileContainer(
                "default",
                new FileContainerConfiguration
                {
                    Provider = provider.Provider,
                    IsMultiTenant = true
                },
                provider,
                currentTenant,
                GetRequiredService<ICancellationTokenProvider>(),
                GetRequiredService<IFileNormalizeNamingService>(),
                GetRequiredService<IServiceProvider>(),
                accessor,
                resolver);

            using (accessor.Change(new FilePathContext { TenantCode = "EXPLICIT" }))
            {
                await container.SaveAsync("a.txt", new MemoryStream([1]), "txt");
            }

            Assert.Equal("EXPLICIT", provider.ContextSeenOnSave!.TenantCode);
            Assert.Equal(0, resolver.CallCount);
            Assert.Null(accessor.Current);
        }

        private class TestFilePathContextResolveContributor : IFilePathContextResolveContributor
        {
            private readonly Action<IFilePathContextResolveContext> _resolveAction;

            public TestFilePathContextResolveContributor(
                string name,
                Action<IFilePathContextResolveContext> resolveAction)
            {
                Name = name;
                _resolveAction = resolveAction;
            }

            public string Name { get; }

            public int CallCount { get; private set; }

            public Task ResolveAsync(IFilePathContextResolveContext context)
            {
                CallCount++;
                _resolveAction(context);
                return Task.CompletedTask;
            }
        }

        private class TestFilePathContextResolver : IFilePathContextResolver
        {
            private readonly FilePathContext? _context;

            public TestFilePathContextResolver(FilePathContext? context)
            {
                _context = context;
            }

            public int CallCount { get; private set; }

            public Task<FilePathContext?> ResolveAsync()
            {
                CallCount++;
                return Task.FromResult(_context);
            }
        }

        private class CapturingFileProvider : IFileProvider
        {
            public string Provider => "Capture";

            public FilePathContext? ContextSeenOnSave { get; private set; }

            public IFilePathContextAccessor FilePathContextAccessor { get; }

            public CapturingFileProvider()
            {
                FilePathContextAccessor = AsyncLocalFilePathContextAccessor.Instance;
            }

            public Task<string> SaveAsync(FileProviderSaveArgs args)
            {
                ContextSeenOnSave = FilePathContextAccessor.Current;
                return Task.FromResult(args.FileId);
            }

            public Task<bool> DeleteAsync(FileProviderDeleteArgs args)
            {
                return Task.FromResult(true);
            }

            public Task<bool> ExistsAsync(FileProviderExistsArgs args)
            {
                return Task.FromResult(true);
            }

            public Task<bool> DownloadAsync(FileProviderDownloadArgs args)
            {
                return Task.FromResult(true);
            }

            public Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
            {
                return Task.FromResult<Stream?>(new MemoryStream());
            }

            public Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
            {
                return Task.FromResult(args.FileId);
            }
        }
    }
}
