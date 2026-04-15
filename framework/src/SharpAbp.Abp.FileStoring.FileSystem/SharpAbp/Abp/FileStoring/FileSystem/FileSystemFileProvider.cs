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
    [ExposeKeyedService<IFileProvider>(FileSystemFileProviderConfigurationNames.ProviderName)]
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

            Logger.LogDebug("Saving file to file system. File: {FilePath}", filePath);

            if (!args.OverrideExisting && await ExistsAsync(filePath))
            {
                throw new FileAlreadyExistsException(
                    $"Saving File '{args.FileId}' already exists in the container '{args.ContainerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            DirectoryHelper.CreateIfNotExists(Path.GetDirectoryName(filePath));

            var fileMode = args.OverrideExisting
                ? FileMode.Create
                : FileMode.CreateNew;

            await Policy.Handle<IOException>()
                .WaitAndRetryAsync(2, retryCount => TimeSpan.FromSeconds(retryCount))
                .ExecuteAsync(async () =>
                {
                    try
                    {
                        using var fileStream = File.Open(filePath, fileMode, FileAccess.Write);
                        await args.FileStream!.CopyToAsync(
                            fileStream,
                            args.CancellationToken
                        );

                        await fileStream.FlushAsync();
                    }
                    catch (IOException ex)
                    {
                        Logger.LogError(ex, "Failed to save file '{FileId}' to file system. Path: {FilePath}", args.FileId, filePath);
                        throw;
                    }
                });

            Logger.LogInformation("Successfully saved file '{FileId}' to file system. Path: {FilePath}", args.FileId, filePath);
            return args.FileId;
        }

        public override Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            Logger.LogDebug("Deleting file from file system. File: {FilePath}", filePath);
            
            var result = FileHelper.DeleteIfExists(filePath);
            
            if (result)
            {
                Logger.LogInformation("Successfully deleted file from file system. File: {FilePath}", filePath);
            }
            
            return Task.FromResult(result);
        }

        public override Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            Logger.LogDebug("Checking file existence in file system. File: {FilePath}", filePath);
            return ExistsAsync(filePath);
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            Logger.LogDebug("Downloading file from file system. File: {FilePath}", filePath);

            if (!File.Exists(filePath))
            {
                Logger.LogWarning("File not found in file system. File: {FilePath}", filePath);
                return false;
            }

            return await Policy.Handle<IOException>()
                .WaitAndRetryAsync(2, retryCount => TimeSpan.FromSeconds(retryCount))
                .ExecuteAsync(async () =>
                {
                    try
                    {
                        using var fileStream = File.OpenRead(filePath);
                        await TryWriteToFileAsync(fileStream, args.Path, args.CancellationToken);
                        Logger.LogInformation("Successfully downloaded file from file system. Source: {SourcePath}, Destination: {DestinationPath}", 
                            filePath, args.Path);
                        return true;
                    }
                    catch (IOException ex)
                    {
                        Logger.LogError(ex, "Failed to download file '{FileId}' from file system. Source: {SourcePath}, Destination: {DestinationPath}", 
                            args.FileId, filePath, args.Path);
                        throw;
                    }
                });
        }

        public override async Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            Logger.LogDebug("Getting file from file system. File: {FilePath}", filePath);
            
            if (!File.Exists(filePath))
            {
                Logger.LogWarning("File not found in file system. File: {FilePath}", filePath);
                return null;
            }

            return await Policy.Handle<IOException>()
                .WaitAndRetryAsync(2, retryCount => TimeSpan.FromSeconds(retryCount))
                .ExecuteAsync(async () =>
                {
                    try
                    {
                        using var fileStream = File.OpenRead(filePath);
                        var result = await TryCopyToMemoryStreamAsync(fileStream, args.CancellationToken);
                        Logger.LogDebug("Successfully retrieved file from file system. File: {FilePath}", filePath);
                        return result;
                    }
                    catch (IOException ex)
                    {
                        Logger.LogError(ex, "Failed to get file '{FileId}' from file system. Path: {FilePath}", args.FileId, filePath);
                        throw;
                    }
                });
        }

        public override async Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                Logger.LogDebug("HTTP access is disabled for file system provider");
                return string.Empty;
            }

            var configuration = args.Configuration.GetFileSystemConfiguration();
            var relativePath = CalculateRelativePath(args);
            var filePath = FilePathCalculator.Calculate(args);
            
            if (args.CheckFileExist && !await ExistsAsync(filePath))
            {
                Logger.LogWarning("File not found when generating access URL. File: {FilePath}", filePath);
                return string.Empty;
            }

            var accessUrl = BuildAccessUrl(configuration, relativePath);
            Logger.LogDebug("Generated access URL for file. File: {FilePath}, URL: {AccessUrl}", filePath, accessUrl);
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
