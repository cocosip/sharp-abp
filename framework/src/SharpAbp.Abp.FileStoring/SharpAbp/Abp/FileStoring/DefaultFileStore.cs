using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring
{
    public class DefaultFileStore : IFileStore, ITransientDependency
    {
        private readonly ILogger _logger;

        private readonly ICurrentTenant _currentTenant;
        public DefaultFileStore(ILogger<DefaultFileStore> logger, ICurrentTenant currentTenant)
        {
            _logger = logger;
            _currentTenant = currentTenant;
        }

        //public async Task<FileIdentity> UploadFileAsync(string filePath)
        //{
        //    return default;
        //}
    }
}
