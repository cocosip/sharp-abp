using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.IO;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring.FileSystem
{
    public class FileSystemFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected ICurrentTenant CurrentTenant { get; }
        protected IFilePathCalculator FilePathCalculator { get; }
        public FileSystemFileProvider(
            ILogger<FileSystemFileProvider> logger,
            ICurrentTenant currentTenant,
            IFilePathCalculator filePathCalculator)
        {
            Logger = logger;
            CurrentTenant = currentTenant;
            FilePathCalculator = filePathCalculator;
        }

        public override string Provider => FileSystemFileProviderConfigurationNames.ProviderName;

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);

            Logger.LogTrace("FileSystem SaveAsync filePath: {filePath}", filePath);

            if (!args.OverrideExisting && await ExistsAsync(filePath))
            {
                throw new FileAlreadyExistsException($"Saving File '{args.FileId}' does already exists in the container '{args.ContainerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            DirectoryHelper.CreateIfNotExists(Path.GetDirectoryName(filePath));

            var fileMode = args.OverrideExisting
                ? FileMode.Create
                : FileMode.CreateNew;

            await Policy.Handle<IOException>()
                .WaitAndRetryAsync(2, retryCount => TimeSpan.FromSeconds(retryCount))
                .ExecuteAsync(async () =>
                {
                    using var fileStream = File.Open(filePath, fileMode, FileAccess.Write);
                    await args.FileStream.CopyToAsync(
                        fileStream,
                        args.CancellationToken
                    );

                    await fileStream.FlushAsync();
                });

            return args.FileId;
        }

        public override Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            Logger.LogTrace("FileSystem DeleteAsync filePath: {filePath}", filePath);
            return Task.FromResult(FileHelper.DeleteIfExists(filePath));
        }

        public override Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            Logger.LogTrace("FileSystem ExistsAsync filePath: {filePath}", filePath);
            return ExistsAsync(filePath);
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            Logger.LogTrace("FileSystem DownloadAsync filePath: {filePath}", filePath);

            if (!File.Exists(filePath))
            {
                return false;
            }

            return await Policy.Handle<IOException>()
                .WaitAndRetryAsync(2, retryCount => TimeSpan.FromSeconds(retryCount))
                .ExecuteAsync(async () =>
                {
                    using var fileStream = File.OpenRead(filePath);
                    await TryWriteToFileAsync(fileStream, args.Path, args.CancellationToken);
                    return true;
                });
        }


        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            Logger.LogTrace("FileSystem GetOrNullAsync filePath: {filePath}", filePath);
            if (!File.Exists(filePath))
            {
                return null;
            }

            return await Policy.Handle<IOException>()
                .WaitAndRetryAsync(2, retryCount => TimeSpan.FromSeconds(retryCount))
                .ExecuteAsync(async () =>
                {
                    using var fileStream = File.OpenRead(filePath);
                    return await TryCopyToMemoryStreamAsync(fileStream, args.CancellationToken);
                });
        }

        public override async Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return string.Empty;
            }

            var configuration = args.Configuration.GetFileSystemConfiguration();
            var relativePath = CalculateRelativePath(args);
            var filePath = FilePathCalculator.Calculate(args);
            if (args.CheckFileExist && !await ExistsAsync(filePath))
            {
                return string.Empty;
            }

            var accessUrl = BuildAccessUrl(configuration, relativePath);
            return accessUrl;
        }


        protected virtual Task<bool> ExistsAsync(string filePath)
        {
            return Task.FromResult(File.Exists(filePath));
        }


        protected virtual string BuildAccessUrl(FileSystemFileProviderConfiguration configuration, string relativePath)
        {
            var accessUrl = $"{configuration.HttpServer.EnsureEndsWith('/')}{relativePath.TrimStart('/')}";
            return accessUrl;
        }

        protected virtual string CalculateRelativePath(FileProviderArgs args)
        {
            var fileSystemConfiguration = args.Configuration.GetFileSystemConfiguration();
            var relativePathBuilder = new StringBuilder();

            if (CurrentTenant.Id == null)
            {
                relativePathBuilder.Append("/host");
            }
            else
            {
                relativePathBuilder.Append($"/tenants/{CurrentTenant.Id.Value:D}");
            }

            if (fileSystemConfiguration.AppendContainerNameToBasePath)
            {
                relativePathBuilder.Append($"/{args.ContainerName}");
            }

            relativePathBuilder.Append($"/{args.FileId}");
            return relativePathBuilder.ToString();
        }
    }
}
