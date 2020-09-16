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

        public FileContainerConfiguration GetConfiguration()
        {
            return _container.GetConfiguration();
        }

        public Task<string> SaveAsync(
            string fileId,
            Stream stream,
            string ext,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default)
        {
            return _container.SaveAsync(
                fileId,
                stream,
                ext,
                overrideExisting,
                cancellationToken
            );
        }

        public Task<bool> DeleteAsync(
            string fileId,
            CancellationToken cancellationToken = default)
        {
            return _container.DeleteAsync(
                fileId,
                cancellationToken
            );
        }

        public Task<bool> ExistsAsync(
            string fileId,
            CancellationToken cancellationToken = default)
        {
            return _container.ExistsAsync(
                fileId,
                cancellationToken
            );
        }

        public Task<bool> DownloadAsync(
            string fileId,
            string path,
            CancellationToken cancellationToken = default)
        {
            return _container.DownloadAsync(
                fileId,
                path,
                cancellationToken
            );
        }

        public Task<Stream> GetAsync(
            string fileId,
            CancellationToken cancellationToken = default)
        {
            return _container.GetAsync(
                fileId,
                cancellationToken
            );
        }

        public Task<Stream> GetOrNullAsync(
            string fileId,
            CancellationToken cancellationToken = default)
        {
            return _container.GetOrNullAsync(
                fileId,
                cancellationToken
            );
        }


        public Task<string> GetAccessUrlAsync(
            string fileId,
            DateTime? expires = null,
            CancellationToken cancellationToken = default)
        {
            return _container.GetAccessUrlAsync(
               fileId,
               expires,
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

        public FileContainerConfiguration GetConfiguration()
        {
            return Configuration;
        }


        public virtual async Task<string> SaveAsync(
            string fileId,
            Stream stream,
            string ext,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default)
        {
            using (CurrentTenant.Change(GetTenantIdOrNull()))
            {
                var (normalizedContainerName, normalizedFileName) = NormalizeNaming(ContainerName, fileId);

                return await Provider.SaveAsync(
                      new FileProviderSaveArgs(
                          normalizedContainerName,
                          Configuration,
                          normalizedFileName,
                          stream,
                          ext,
                          overrideExisting,
                          CancellationTokenProvider.FallbackToProvider(cancellationToken)
                      )
                  );
            }
        }

        public virtual async Task<bool> DeleteAsync(
            string fileId,
            CancellationToken cancellationToken = default)
        {
            using (CurrentTenant.Change(GetTenantIdOrNull()))
            {
                var (normalizedContainerName, normalizedFileName) =
                    NormalizeNaming(ContainerName, fileId);

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
            string fileId,
            CancellationToken cancellationToken = default)
        {
            using (CurrentTenant.Change(GetTenantIdOrNull()))
            {
                var (normalizedContainerName, normalizedFileName) =
                    NormalizeNaming(ContainerName, fileId);

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

        public virtual async Task<bool> DownloadAsync(
            string fileId,
            string path,
            CancellationToken cancellationToken = default
        )
        {
            using (CurrentTenant.Change(GetTenantIdOrNull()))
            {
                var (normalizedContainerName, normalizedFileName) =
                    NormalizeNaming(ContainerName, fileId);

                return await Provider.DownloadAsync(
                     new FileProviderDownloadArgs(
                         normalizedContainerName,
                         Configuration,
                         normalizedFileName,
                         path,
                         CancellationTokenProvider.FallbackToProvider(cancellationToken)
                     )
                 );
            }

        }

        public virtual async Task<Stream> GetAsync(
            string fileId,
            CancellationToken cancellationToken = default)
        {
            var stream = await GetOrNullAsync(fileId, cancellationToken);

            if (stream == null)
            {
                //TODO: Consider to throw some type of "not found" exception and handle on the HTTP status side
                throw new AbpException(
                    $"Could not found the requested FILE '{fileId}' in the container '{ContainerName}'!");
            }

            return stream;
        }


        public virtual async Task<Stream> GetOrNullAsync(
            string fileId,
            CancellationToken cancellationToken = default)
        {
            using (CurrentTenant.Change(GetTenantIdOrNull()))
            {
                var (normalizedContainerName, normalizedFileName) =
                    NormalizeNaming(ContainerName, fileId);

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

        public virtual async Task<string> GetAccessUrlAsync(
            string fileId,
            DateTime? expires = null,
            CancellationToken cancellationToken = default)
        {
            using (CurrentTenant.Change(GetTenantIdOrNull()))
            {
                var (normalizedContainerName, normalizedFileName) =
                    NormalizeNaming(ContainerName, fileId);

                return await Provider.GetAccessUrlAsync(
                    new FileProviderAccessArgs(
                        normalizedContainerName,
                        Configuration,
                        normalizedFileName,
                        expires,
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
                    fileId = normalizer.NormalizeFileName(fileId);
                }

                return (containerName, fileId);
            }
        }


    }
}
