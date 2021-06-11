namespace SharpAbp.Abp.Snowflakes
{
    public class SnowflakeConfiguration
    {
        /// <summary>
        /// 开始时间截(2015-01-01)
        /// </summary>
        public long Twepoch { get; set; } = 1420041600000L;

        /// <summary>
        /// 机器id所占的位数
        /// </summary>
        public int WorkerIdBits { get; set; } = 5;

        /// <summary>
        /// 数据标识id所占的位数
        /// </summary>
        public int DatacenterIdBits { get; set; } = 5;

        /// <summary>
        /// 序列在id中占的位数(1ms内的并发数)
        /// </summary>
        public int SequenceBits { get; set; } = 12;

        /// <summary>
        /// 机器id
        /// </summary>
        public long WorkerId { get; set; } = 0L;

        /// <summary>
        /// 数据中心id
        /// </summary>
        public long DatacenterId { get; set; } = 0L;
    }
}
