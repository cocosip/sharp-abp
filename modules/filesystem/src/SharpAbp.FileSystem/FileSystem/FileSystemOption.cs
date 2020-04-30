namespace SharpAbp.FileSystem
{
    /// <summary>文件存取的配置
    /// </summary>
    public class FileSystemOption
    {
        /// <summary>在上传之前是否启用文件id检测机制
        /// </summary>
        public bool EnableFileIdCheck { get; set; } = false;

        /// <summary>文件Id重复策略
        /// </summary>
        public FileIdRepeatPolicy FileIdRepeatPolicy { get; set; } = FileIdRepeatPolicy.Cover;

    }

    /// <summary>重复文件Id策略
    /// </summary>
    public enum FileIdRepeatPolicy
    {
        /// <summary>覆盖
        /// </summary>
        Cover = 1,

        /// <summary>生成新的文件Id
        /// </summary>
        GenerateNew = 2,

        /// <summary>跳过
        /// </summary>
        Skip = 4

    }
}
