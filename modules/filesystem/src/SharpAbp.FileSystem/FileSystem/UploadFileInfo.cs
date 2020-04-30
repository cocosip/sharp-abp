using System.Collections.Generic;

namespace SharpAbp.FileSystem
{
    /// <summary>上传的文件信息
    /// </summary>
    public class UploadFileInfo
    {
        /// <summary>文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>文件流
        /// </summary>
        public byte[] FileData { get; set; }

        /// <summary>文件扩展名
        /// </summary>
        public string FileExt { get; set; }

        /// <summary>扩展信息
        /// </summary>
        public List<KeyValuePair<string, string>> Expands { get; set; } = new List<KeyValuePair<string, string>>();

    }
}
