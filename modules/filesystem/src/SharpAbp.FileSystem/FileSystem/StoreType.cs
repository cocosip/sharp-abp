namespace SharpAbp.FileSystem
{
    /// <summary>存储类型
    /// </summary>
    public enum StoreType
    {
        /// <summary>S3存储
        /// </summary>
        S3 = 1,

        /// <summary>基于FastDFS的存储
        /// </summary>
        FastDFS = 2,

        /// <summary>基于存储柜等网络存储
        /// </summary>
        NetDFS = 4
    }
}
