using System;
using System.IO;
using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoring.Fakes
{
    public class FakeFileProvider2 : IFileProvider
    {
        public Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            throw new NotImplementedException();
        }

        public Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
