namespace SharpAbp.FileSystem
{
    /// <summary>文件信息描述
    /// </summary>
    public class FileDescriptor
    {
        /// <summary>医院编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>存储类型
        /// </summary>
        public StoreType StoreType { get; set; }

        /// <summary>组名或者Buckt名称
        /// </summary>
        public string PatchName { get; set; }

        /// <summary>文件FileId
        /// </summary>
        public string FileId { get; set; }

        /// <summary>文件扩展名
        /// </summary>
        public string FileExt { get; set; }

    }
}
