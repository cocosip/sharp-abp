using System.Collections.Generic;

namespace SharpAbp.FileSystem
{
    /// <summary>Patch信息
    /// </summary>
    public class PatchInfo
    {
        /// <summary>存储类型
        /// </summary>
        public StoreType StoreType { get; set; }

        /// <summary>Patch名(OSS中为Bucket名,FastDFS中为组名,NFS中为服务器的信息)
        /// </summary>
        public string Name { get; set; }

        /// <summary>编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>状态
        /// </summary>
        public int State { get; set; }

        /// <summary>扩展属性
        /// </summary>
        public List<List<KeyValuePair<string, string>>> Expands { get; set; } = new List<List<KeyValuePair<string, string>>>();


    }
}
