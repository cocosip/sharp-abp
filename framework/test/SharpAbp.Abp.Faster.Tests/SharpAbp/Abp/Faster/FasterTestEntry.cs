#nullable enable

namespace SharpAbp.Abp.Faster
{
    [FasterLogName("faster-test-entry")]
    public class FasterTestEntry
    {
        public int Id { get; set; }

        public string? Value { get; set; }
    }
}
