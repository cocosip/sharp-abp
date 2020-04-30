using System;
using System.Threading.Tasks;

namespace SharpAbp.FileSystem
{
    /// <summary>文件存储客户端接口
    /// </summary>
    public interface IFileStoreClient
    {
        /// <summary>上传文件
        /// </summary>
        Task<FileIdentifier> UploadFileAsync(Patch patch, UploadFileInfo info);

        /// <summary>下载文件到指定目录
        /// </summary>
        Task DownloadFileAsync(FileIdentifier fileIdentifier, string savePath);

        /// <summary>获取文件二进制
        /// </summary>
        Task<byte[]> GetFileAsync(FileIdentifier fileIdentifier);

        /// <summary>删除文件
        /// </summary>
        Task DeleteFileAsync(FileIdentifier fileIdentifier);

        /// <summary>获取文件的信息
        /// </summary>
        Task<FileMetaInfo> GetMetaInfoAsync(FileIdentifier fileIdentifier);

        /// <summary>获取文件的访问地址
        /// </summary>
        string GetUrl(FileIdentifier fileIdentifier, DateTime? expires = null);
    }
}
