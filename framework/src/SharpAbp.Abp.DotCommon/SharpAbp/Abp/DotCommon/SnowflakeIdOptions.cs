namespace SharpAbp.Abp.DotCommon
{
    public class SnowflakeIdOptions
    {
        public long WorkerId { get; set; } = 1;

        public long DatacenterId { get; set; } = 1;
    }
}
