using System;
using System.Threading.Tasks;

namespace SharpAbp.FileSystem
{
    /// <summary>通用文件存储类
    /// </summary>
    public interface IFileStore
    {

        /// <summary>上传文件
        /// </summary>
        Task<FileIdentifier> UploadFileAsync(string code, UploadFileInfo info);


        /// <summary>下载文件到指定路径
        /// </summary>
        /// <param name="code">医院编码</param>
        /// <param name="storeType">存储类型</param>
        /// <param name="patchName">Patch名</param>
        /// <param name="fileId">文件Id</param>
        /// <param name="savePath">保存路径</param>
        /// <returns></returns>
        Task DownloadToFileAsync(string code, StoreType storeType, string patchName, string fileId, string savePath);

        /// <summary>下载文件到指定路径
        /// </summary>
        /// <param name="descriptor">文件描述</param>
        /// <param name="savePath">保存路径</param>
        /// <returns></returns>
        Task DownloadToFileAsync(FileDescriptor descriptor, string savePath);

        /// <summary>获取文件数据
        /// </summary>
        /// <param name="code">医院编码</param>
        /// <param name="storeType">存储类型</param>
        /// <param name="patchName">Patch名</param>
        /// <param name="fileId">文件Id</param>
        /// <returns></returns>
        Task<byte[]> GetFileAsync(string code, StoreType storeType, string patchName, string fileId);

        /// <summary>根据文件信息描述获取文件数据
        /// </summary>
        /// <param name="descriptor">文件描述</param>
        /// <returns></returns>
        Task<byte[]> GetFileAsync(FileDescriptor descriptor);

        /// <summary>删除文件
        /// </summary>
        /// <param name="code">医院编码</param>
        /// <param name="storeType">存储类型</param>
        /// <param name="patchName">Patch名</param>
        /// <param name="fileId">文件Id</param>
        /// <returns></returns>
        Task DeleteFileAsync(string code, StoreType storeType, string patchName, string fileId);

        /// <summary>获取文件数据
        /// </summary>
        Task<FileMetaInfo> GetMetaInfoAsync(string code, StoreType storeType, string patchName, string fileId);

        /// <summary>获取文件数据
        /// </summary>
        Task<FileMetaInfo> GetMetaInfoAsync(FileDescriptor fileDescriptor);

        /// <summary>生成文件Id
        /// </summary>
        Task<FileIdentifier> BuildFileIdentifier(string code, StoreType storeType, string patchName, string fileId);

        /// <summary>获取访问的Url地址
        /// </summary>
        Task<string> GetAccessUrl(string code, StoreType storeType, string patchName, string fileId, DateTime? expires = null);
    }
}
