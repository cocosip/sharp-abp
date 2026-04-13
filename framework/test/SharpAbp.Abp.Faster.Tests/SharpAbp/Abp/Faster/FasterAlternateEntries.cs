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
}
