using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpAbp.FileSystem.FastDFS;
using SharpAbp.FileSystem.S3;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SharpAbp.FileSystem.Impl
{
    /// <summary>通用文件存储类
    /// </summary>
    public class DefaultFileStore : IFileStore
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        private readonly IFileSystemQuery _fileSystemQuery;
        private readonly IPatchSelector _patchSelector;
        public DefaultFileStore(ILogger<DefaultFileStore> logger, IServiceProvider provider, IFileSystemQuery fileSystemQuery, IPatchSelector patchSelector)
        {
            _logger = logger;
            _provider = provider;
            _fileSystemQuery = fileSystemQuery;
            _patchSelector = patchSelector;
        }

        /// <summary>上传文件
        /// </summary>
        public async Task<FileIdentifier> UploadFileAsync(string code, UploadFileInfo info)
        {
            //查询出全部的Patch
            var patchs = await _fileSystemQuery.FindPatchsAsync(code);
            //选中的Patch
            var selectedPatch = _patchSelector.SelectPatch(code, patchs);
            var client = GetClient(code, selectedPatch);

            return await client.UploadFileAsync(selectedPatch, info);
        }

        /// <summary>删除文件
        /// </summary>
        /// <param name="code">医院编码</param>
        /// <param name="storeType">存储类型</param>
        /// <param name="patchName">Patch名</param>
        /// <param name="fileId">文件Id</param>
        /// <returns></returns>
        public async Task DeleteFileAsync(string code, StoreType storeType, string patchName, string fileId)
        {
            var fileIdentifier = await BuildFileIdentifier(code, storeType, patchName, fileId);

            var client = GetClient(code, fileIdentifier.Patch);
            await client.DeleteFileAsync(fileIdentifier);
        }

        /// <summary>下载文件到指定路径
        /// </summary>
        /// <param name="code">医院编码</param>
        /// <param name="storeType">存储类型</param>
        /// <param name="patchName">Patch名</param>
        /// <param name="fileId">文件Id</param>
        /// <param name="savePath">保存路径</param>
        /// <returns></returns>
        public async Task DownloadToFileAsync(string code, StoreType storeType, string patchName, string fileId, string savePath)
        {
            var fileIdentifier = await BuildFileIdentifier(code, storeType, patchName, fileId);
            var client = GetClient(code, fileIdentifier.Patch);
            await client.DownloadFileAsync(fileIdentifier, savePath);
        }

        /// <summary>下载文件到指定路径
        /// </summary>
        /// <param name="descriptor">文件描述</param>
        /// <param name="savePath">保存路径</param>
        /// <returns></returns>
        public async Task DownloadToFileAsync(FileDescriptor descriptor, string savePath)
        {
            await DownloadToFileAsync(descriptor.Code, descriptor.StoreType, descriptor.PatchName, descriptor.FileId, savePath);
        }


        /// <summary>获取文件数据
        /// </summary>
        /// <param name="code">医院编码</param>
        /// <param name="storeType">存储类型</param>
        /// <param name="patchName">Patch名</param>
        /// <param name="fileId">文件Id</param>
        /// <returns></returns>
        public async Task<byte[]> GetFileAsync(string code, StoreType storeType, string patchName, string fileId)
        {
            var fileIdentifier = await BuildFileIdentifier(code, storeType, patchName, fileId);
            var client = GetClient(code, fileIdentifier.Patch);
            return await client.GetFileAsync(fileIdentifier);
        }

        /// <summary>根据文件信息描述获取文件数据
        /// </summary>
        /// <param name="descriptor">文件描述</param>
        /// <returns></returns>
        public async Task<byte[]> GetFileAsync(FileDescriptor descriptor)
        {
            return await GetFileAsync(descriptor.Code, descriptor.StoreType, descriptor.PatchName, descriptor.FileId);
        }

        /// <summary>根据文件信息描述获取文件信息
        /// </summary>
        /// <param name="descriptor">文件描述</param>
        /// <returns></returns>
        public async Task<FileMetaInfo> GetMetaInfoAsync(FileDescriptor fileDescriptor)
        {
            return await GetMetaInfoAsync(fileDescriptor.Code, fileDescriptor.StoreType, fileDescriptor.PatchName, fileDescriptor.FileId);
        }


        /// <summary>根据文件信息描述获取文件信息
        /// </summary>
        /// <param name="code">医院编码</param>
        /// <param name="storeType">存储类型</param>
        /// <param name="patchName">Patch名</param>
        /// <param name="fileId">文件Id</param>
        /// <returns></returns>
        public async Task<FileMetaInfo> GetMetaInfoAsync(string code, StoreType storeType, string patchName, string fileId)
        {
            var fileIdentifier = await BuildFileIdentifier(code, storeType, patchName, fileId);
            var client = GetClient(code, fileIdentifier.Patch);
            return await client.GetMetaInfoAsync(fileIdentifier);
        }


        /// <summary>生成文件Id
        /// </summary>
        public async Task<FileIdentifier> BuildFileIdentifier(string code, StoreType storeType, string patchName, string fileId)
        {
            var patch = await _fileSystemQuery.FindPatchByPatchNameAsync(code, storeType, patchName);

            var fileIdentifier = new FileIdentifier()
            {
                FileId = fileId,
                Patch = patch,
                StoreType = (StoreType) patch.StoreType
            };
            return fileIdentifier;
        }


        /// <summary>获取访问的Url地址
        /// </summary>
        public async Task<string> GetAccessUrl(string code, StoreType storeType, string patchName, string fileId, DateTime? expires = null)
        {
            var fileIdentifier = await BuildFileIdentifier(code, storeType, patchName, fileId);
            var client = GetClient(code, fileIdentifier.Patch);
            return client.GetUrl(fileIdentifier, expires);
        }


        private IFileStoreClient GetClient(string code, Patch patch)
        {
            IFileStoreClient client = null;
            //选择上传的客户端
            var services = _provider.GetServices<IFileStoreClient>();
            if (patch.StoreType == (int) StoreType.S3)
            {
                client = services.FirstOrDefault(x => x is S3FileStoreClient);
            }
            else if (patch.StoreType == (int) StoreType.FastDFS)
            {
                client = services.FirstOrDefault(x => x is FastDFSFileStoreClient);
            }

            if (client == null)
            {
                throw new ArgumentException($"未找到存储客户端,医院编码:{code},存储类型:{patch.StoreType.ToString()}");
            }
            return client;
        }


    }
}
