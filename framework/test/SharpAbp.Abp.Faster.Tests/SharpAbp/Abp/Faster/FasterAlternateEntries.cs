namespace SharpAbp.Abp.Faster
{
    public class FasterAlternateEntryA
    {
        public int Id { get; set; }
    }

    public class FasterAlternateEntryB
    {
        public int Id { get; set; }
    }

    [FasterLogName("faster-concurrent-test-entry")]
    public class FasterConcurrentTestEntry
    {
        public int Id { get; set; }
    }

    [FasterLogName("faster-late-commit-test-entry")]
    public class FasterLateCommitTestEntry
    {
        public int Id { get; set; }
    }

    public class FasterGenericPayloadEntry<T>
    {
        public T Payload { get; set; }
    }

    [FasterLogName("faster-range-limit-test-entry")]
    public class FasterRangeLimitTestEntry
    {
        public int Id { get; set; }
    }

    public class FasterRecoverableInitEntry
    {
        public int Id { get; set; }
    }
}
