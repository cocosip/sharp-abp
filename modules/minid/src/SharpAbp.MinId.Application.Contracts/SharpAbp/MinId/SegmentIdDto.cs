namespace SharpAbp.MinId
{
    public class SegmentIdDto
    {
        public long CurrentId { get; set; }
        public long MaxId { get; set; }
        public int Delta { get; set; }
        public int Remainder { get; set; }
        public long LoadingId { get; set; }
    }
}
