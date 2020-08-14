using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring
{
    public class DefaultFileStore : IFileStore, ITransientDependency
    {
        private readonly ILogger _logger;

        public DefaultFileStore(ILogger<DefaultFileStore> logger)
        { 
            _logger = logger;
        }

        //public async Task<FileIdentity> UploadFileAsync(string filePath)
        //{
        //    return default;
        //}
    }
}
