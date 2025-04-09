﻿using System.IO;
using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoring
{
    public interface IFileProvider
    {
        string Provider { get; }

        Task<string> SaveAsync(FileProviderSaveArgs args);

        Task<bool> DeleteAsync(FileProviderDeleteArgs args);

        Task<bool> ExistsAsync(FileProviderExistsArgs args);

        Task<bool> DownloadAsync(FileProviderDownloadArgs args);

        Task<Stream?> GetOrNullAsync(FileProviderGetArgs args);

        Task<string> GetAccessUrlAsync(FileProviderAccessArgs args);
    }
}