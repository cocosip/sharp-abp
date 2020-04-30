using DotCommon.Utility;
using FastDFSCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SharpAbp.FileSystem.FastDFS
{
    /// <summary>FastDFS文件存储
    /// </summary>
    public class FastDFSFileStoreClient : IFileStoreClient
    {
        private readonly ILogger _logger;
        private readonly IFDFSClient _fdfsClient;

        /// <summary>Ctor
        /// </summary>
        public FastDFSFileStoreClient(ILogger<FastDFSFileStoreClient> logger, IFDFSClient fdfsClient)
        {
            _logger = logger;
            _fdfsClient = fdfsClient;
        }

        /// <summary>上传文件
        /// </summary>
        public async Task<FileIdentifier> UploadFileAsync(Patch patch, UploadFileInfo info)
        {
            var fastDFSPatch = (FastDFSPatch) patch;
            //当前存储的Group
            var storageNode = await _fdfsClient.GetStorageNodeAsync(fastDFSPatch.Name);
            string fileId;
            if (info.FileData != null)
            {
                fileId = await _fdfsClient.UploadFileAsync(storageNode, info.FileData, info.FileExt);
            }
            else
            {
                fileId = await _fdfsClient.UploadFileAsync(storageNode, info.FilePath);
            }
            var fileIdentifier = new FileIdentifier()
            {
                FileId = fileId,
                Patch = patch,
                StoreType = StoreType.FastDFS
            };
            return fileIdentifier;
        }

        /// <summary>删除文件
        /// </summary>
        public async Task DeleteFileAsync(FileIdentifier fileIdentifier)
        {
            var fastDFSPatch = (FastDFSPatch) fileIdentifier.Patch;
            await _fdfsClient.RemoveFileAsync(fastDFSPatch.Name, fileIdentifier.FileId);
        }

        /// <summary>下载文件到指定目录
        /// </summary>
        public async Task DownloadFileAsync(FileIdentifier fileIdentifier, string savePath)
        {
            var storageNode = await _fdfsClient.GetStorageNodeAsync(fileIdentifier.Patch.Name);
            _ = await _fdfsClient.DownloadFileEx(storageNode, fileIdentifier.FileId, savePath);
        }

        /// <summary>获取文件二进制
        /// </summary>
        public async Task<byte[]> GetFileAsync(FileIdentifier fileIdentifier)
        {
            var storageNode = await _fdfsClient.GetStorageNodeAsync(fileIdentifier.Patch.Name);
            return await _fdfsClient.DownloadFileAsync(storageNode, fileIdentifier.FileId);
        }

        /// <summary>获取文件的信息
        /// </summary>
        public async Task<FileMetaInfo> GetMetaInfoAsync(FileIdentifier fileIdentifier)
        {
            var fileMeta = new FileMetaInfo()
            {
                FileId = fileIdentifier.FileId
            };

            var storageNode = await _fdfsClient.GetStorageNodeAsync(fileIdentifier.Patch.Name);
            var fileInfo = await _fdfsClient.GetFileInfo(storageNode, fileIdentifier.FileId);
            fileMeta.FileSize = fileInfo.FileSize;
            return fileMeta;

        }

        /// <summary>获取访问的Url地址
        /// </summary>
        public string GetUrl(FileIdentifier fileIdentifier, DateTime? expires = null)
        {
            var fastDFSPatch = (FastDFSPatch) fileIdentifier.Patch;
            var rd = new Random();
            var index = rd.Next(fastDFSPatch.Trackers.Count);
            var selectTracker = fastDFSPatch.Trackers[index];
            return UrlUtil.CombineUrl(selectTracker.Url, fastDFSPatch.Name, fileIdentifier.FileId);
            //return $"{selectTracker.Url.TrimEnd('/')}/{fastDFSPatch.Name}/{fileIdentifier.FileId.TrimStart('/')}";
        }




    }
}
