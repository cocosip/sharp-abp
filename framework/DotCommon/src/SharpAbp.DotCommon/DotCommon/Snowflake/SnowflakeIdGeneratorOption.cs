namespace SharpAbp.DotCommon.Snowflake
{
    /// <summary>雪花算法Id生成器配置
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
