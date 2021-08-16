namespace SharpAbp.Abp.DotCommon.Performance
{
    public class PerformanceInfo
    {
        public long TotalCount { get; private set; }
        public long Throughput { get; private set; }
        public long AverageThroughput { get; private set; }
        public double RT { get; private set; }
        public double AverageRT { get; private set; }

        public PerformanceInfo(long totalCount, long throughput, long averageThroughput, double rt, double averageRT)
        {
            TotalCount = totalCount;
            Throughput = throughput;
            AverageThroughput = averageThroughput;
            RT = rt;
            AverageRT = averageRT;
        }
    }
}
