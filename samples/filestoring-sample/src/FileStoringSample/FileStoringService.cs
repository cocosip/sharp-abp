using Microsoft.Extensions.Logging;
using SharpAbp.Abp.FileStoring;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace FileStoringSample
{
    public class FileStoringService : ITransientDependency
    {
        private readonly List<string> _containers = new()
        {
            //"aliyun-container",
            "filesystem-container",
            "minio-container",
            "s3-container"
        };

        private readonly ILogger _logger;
        private readonly IFileContainerFactory _fileContainerFactory;
        public FileStoringService(ILogger<FileStoringService> logger, IFileContainerFactory fileContainerFactory)
        {
            _logger = logger;
            _fileContainerFactory = fileContainerFactory;
        }

        public async Task RunContainers()
        {

            await Task.Run(async () =>
            {
                foreach (var name in _containers)
                {
                    try
                    {
                        await RunContainerTest(name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Test container '{0}' failed:{1}.", name, ex.Message);
                    }
                }
            });
        }

        private async Task RunContainerTest(string name)
        {
            var fileId = await UploadAsync(name);
            await GetAccessUrlAsync(name, fileId);
            var downloadPath = await DownloadAsync(name, fileId);
            await DeleteAsync(name, fileId);

            if (File.Exists(downloadPath))
            {
                File.Delete(downloadPath);
            }
        }

        private async Task<string> UploadAsync(string name)
        {
            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "files", "test.txt");
                var container = _fileContainerFactory.Create(name);
                var fileId = await container.SaveAsync($"000/{Guid.NewGuid()}.txt", filePath);
                _logger.LogDebug("[{0}] Upload fileId:{1}", name, fileId);
                return fileId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload failed:{0}.", ex.Message);
            }

            return "";
        }

        private async Task<string> DownloadAsync(string name, string fileId)
        {
            try
            {
                var dir = Path.Combine(AppContext.BaseDirectory, "test");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var container = _fileContainerFactory.Create(name);
                var downloadPath = Path.Combine(dir, $"{Guid.NewGuid()}.txt");
                var downloadResult = await container.DownloadAsync(fileId, downloadPath);
                _logger.LogDebug("[{0}] Download fileId:{1} save at:'{2}',result:{3}", name, fileId, downloadPath, downloadResult);
                return downloadPath;
            }
            catch (Exception ex)
            {
                _logger.LogError("Download failed:{0}.", ex.Message);
            }
            return "";
        }

        private async Task GetAccessUrlAsync(string name, string fileId)
        {
            try
            {
                var container = _fileContainerFactory.Create(name);
                var url = await container.GetAccessUrlAsync(fileId);
                _logger.LogDebug("[{0}] GetAccessUrl by fileId:{1},Url:{2}.", name, fileId, url);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetAccessUrl failed:{0}.", ex.Message);
            }
        }

        private async Task DeleteAsync(string name, string fileId)
        {
            try
            {
                var container = _fileContainerFactory.Create(name);
                var result = await container.DeleteAsync(fileId);
                _logger.LogDebug("[{0}] Delete fileId:{1},result:{2}.", name, fileId, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete failed:{0}.", ex.Message);
            }
        }


    }
}
