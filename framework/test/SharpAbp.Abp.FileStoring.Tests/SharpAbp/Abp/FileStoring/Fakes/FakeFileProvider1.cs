using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.Fakes
{
    [ExposeKeyedService<IFileProvider>(nameof(FakeFileProvider1))]
    public class FakeFileProvider1 : IFileProvider, ITransientDependency
    {
        public string Provider => nameof(FakeFileProvider1);

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
