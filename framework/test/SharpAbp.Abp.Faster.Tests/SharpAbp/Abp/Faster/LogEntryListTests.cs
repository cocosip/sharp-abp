using Xunit;

namespace SharpAbp.Abp.Faster
{
    public class LogEntryListTests
    {
        [Fact]
        public void GetPositions_ReturnsCorrectPositions()
        {
            var list = new LogEntryList<FasterTestEntry>
            {
                new LogEntry<FasterTestEntry> { Data = new FasterTestEntry { Id = 1 }, CurrentAddress = 100, NextAddress = 200 },
                new LogEntry<FasterTestEntry> { Data = new FasterTestEntry { Id = 2 }, CurrentAddress = 200, NextAddress = 350 },
            };

            var positions = list.GetPositions();

            Assert.Equal(2, positions.Count);
            Assert.Equal(100, positions[0].Address);
            Assert.Equal(200, positions[0].NextAddress);
            Assert.Equal(200, positions[1].Address);
            Assert.Equal(350, positions[1].NextAddress);
        }

        [Fact]
        public void GetPositions_EmptyList_ReturnsEmptyList()
        {
            var list = new LogEntryList<FasterTestEntry>();
            var positions = list.GetPositions();
            Assert.Empty(positions);
        }

        [Fact]
        public void LogEntry_EntryLength_IsCorrect()
        {
            var entry = new LogEntry<FasterTestEntry>
            {
                CurrentAddress = 100,
                NextAddress = 250
            };
            Assert.Equal(150, entry.EntryLength);
        }
    }
}
