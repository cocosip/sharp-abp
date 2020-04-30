namespace SharpAbp.DotCommon.Snowflake
{
    /// <summary>雪花Id算法配置选项
    /// </summary>
    public class SnowflakeIdGeneratorOption
    {
        /// <summary>数据中心Id
        /// </summary>
        public long DataCenterId { get; set; }

        /// <summary>WorkerId
        /// </summary>
        public long WorkerId { get; set; }
    }
}
