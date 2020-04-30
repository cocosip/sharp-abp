namespace SharpAbp.FileSystem.FastDFS
{
    /// <summary>Tracker信息
    /// </summary>
    public class Tracker
    {
        /// <summary>名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>IP地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>Url地址
        /// </summary>
        public string Url { get; set; }
    }
}
