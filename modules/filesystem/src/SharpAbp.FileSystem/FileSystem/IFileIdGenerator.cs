namespace SharpAbp.FileSystem
{
    /// <summary>
    /// </summary>
    public interface IFileIdGenerator
    {
        /// <summary>默认的存储路径生成器
        /// </summary>
        string GenerateFileId(StoreType storeType, Patch patch, UploadFileInfo info);
    }
}
