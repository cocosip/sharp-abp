using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FileStoring
{
    public class FileContainer<TContainer> : IFileContainer<TContainer>
        where TContainer : class
    {
        private readonly IFileContainer _container;

        public FileContainer(IFileContainerFactory fileContainerFactory)
        {
            _container = fileContainerFactory.Create<TContainer>();
        }

        public Task SaveAsync(
            string name,
            Stream stream,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default)
        {
            return _container.SaveAsync(
                name,
                stream,
                overrideExisting,
                cancellationToken
            );
        }

        public Task<bool> DeleteAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            return _container.DeleteAsync(
                name,
                cancellationToken
            );
        }

        public Task<bool> ExistsAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            return _container.ExistsAsync(
                name,
                cancellationToken
            );
        }

        public Task<Stream> GetAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            return _container.GetAsync(
                name,
                cancellationToken
            );
        }

        public Task<Stream> GetOrNullAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            return _container.GetOrNullAsync(
                name,
                cancellationToken
            );
        }
    }

    public class FileContainer : IFileContainer
    {
        protected string ContainerName { get; }

        protected FileContainerConfiguration Configuration { get; }

        protected IFileProvider Provider { get; }

        protected ICurrentTenant CurrentTenant { get; }

        protected ICancellationTokenProvider CancellationTokenProvider { get; }

        protected IServiceProvider ServiceProvider { get; }

        public FileContainer(
            string containerName,
            FileContainerConfiguration configuration,
            IFileProvider provider,
            ICurrentTenant currentTenant,
            ICancellationTokenProvider cancellationTokenProvider,
            IServiceProvider serviceProvider)
        {
            ContainerName = containerName;
            Configuration = configuration;
            Provider = provider;
            CurrentTenant = currentTenant;
            CancellationTokenProvider = cancellationTokenProvider;
            ServiceProvider = serviceProvider;
        }


        public virtual async Task SaveAsync(
            string fileId,
            Stream stream,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default)
        {
            using (CurrentTenant.Change(GetTenantIdOrNull()))
            {
                var (normalizedContainerName, normalizedFileName) = NormalizeNaming(ContainerName, fileId);

                await Provider.SaveAsync(
                    new FileProviderSaveArgs(
                        normalizedContainerName,
                        Configuration,
                        normalizedFileName,
                        stream,
                        "",
                        overrideExisting,
                        CancellationTokenProvider.FallbackToProvider(cancellationToken)
                    )
                );
            }
        }

        public virtual async Task<bool> DeleteAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            using (CurrentTenant.Change(GetTenantIdOrNull()))
            {
                var (normalizedContainerName, normalizedFileName) =
                    NormalizeNaming(ContainerName, name);

                return await Provider.DeleteAsync(
                    new FileProviderDeleteArgs(
                        normalizedContainerName,
                        Configuration,
                        normalizedFileName,
                        CancellationTokenProvider.FallbackToProvider(cancellationToken)
                    )
                );
            }
        }

        public virtual async Task<bool> ExistsAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            using (CurrentTenant.Change(GetTenantIdOrNull()))
            {
                var (normalizedContainerName, normalizedFileName) =
                    NormalizeNaming(ContainerName, name);

                return await Provider.ExistsAsync(
                    new FileProviderExistsArgs(
                        normalizedContainerName,
                        Configuration,
                        normalizedFileName,
                        CancellationTokenProvider.FallbackToProvider(cancellationToken)
                    )
                );
            }
        }

        public virtual async Task<Stream> GetAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            var stream = await GetOrNullAsync(name, cancellationToken);

            if (stream == null)
            {
                //TODO: Consider to throw some type of "not found" exception and handle on the HTTP status side
                throw new AbpException(
                    $"Could not found the requested FILE '{name}' in the container '{ContainerName}'!");
            }

            return stream;
        }

        public virtual async Task<Stream> GetOrNullAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            using (CurrentTenant.Change(GetTenantIdOrNull()))
            {
                var (normalizedContainerName, normalizedFileName) =
                    NormalizeNaming(ContainerName, name);

                return await Provider.GetOrNullAsync(
                    new FileProviderGetArgs(
                        normalizedContainerName,
                        Configuration,
                        normalizedFileName,
                        CancellationTokenProvider.FallbackToProvider(cancellationToken)
                    )
                );
            }
        }

        protected virtual Guid? GetTenantIdOrNull()
        {
            if (!Configuration.IsMultiTenant)
            {
                return null;
            }

            return CurrentTenant.Id;
        }

        protected virtual (string, string) NormalizeNaming(string containerName, string fileId)
        {
            if (!Configuration.NamingNormalizers.Any())
            {
                return (containerName, fileId);
            }

            using (var scope = ServiceProvider.CreateScope())
            {
                foreach (var normalizerType in Configuration.NamingNormalizers)
                {
                    var normalizer = scope.ServiceProvider
                        .GetRequiredService(normalizerType)
                        .As<IFileNamingNormalizer>();

                    containerName = normalizer.NormalizeContainerName(containerName);
                    fileId = normalizer.NormalizeFileId(fileId);
                }

                return (containerName, fileId);
            }
        }
    }
}
