using OBS;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public class ObsFileProvider : FileProviderBase, ITransientDependency
    {
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }

        public ObsFileProvider()
        {

        }

        public override string Provider => ObsFileProviderConfigurationNames.ProviderName;

        public override Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            throw new NotImplementedException();
        }

        public override Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            throw new NotImplementedException();
        }

        public override Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            throw new NotImplementedException();
        }

        public override Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
