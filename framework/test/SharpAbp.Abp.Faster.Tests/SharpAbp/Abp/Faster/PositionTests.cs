using System;
using Xunit;

namespace SharpAbp.Abp.Faster
{
    public class PositionTests
    {
        [Fact]
        public void Constructor_ValidArguments_ShouldSucceed()
        {
            var pos = new Position(100, 200);
            Assert.Equal(100, pos.Address);
            Assert.Equal(200, pos.NextAddress);
            Assert.Equal(100, pos.Length);
        }

        [Fact]
        public void Constructor_NegativeAddress_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new Position(-1, 100));
        }

        [Fact]
        public void Constructor_NextAddressNotGreaterThanAddress_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new Position(100, 100));
            Assert.Throws<ArgumentException>(() => new Position(100, 50));
        }

        [Fact]
        public void IsValid_ValidPosition_ReturnsTrue()
        {
            var pos = new Position(0, 1);
            Assert.True(pos.IsValid());
        }

        [Fact]
        public void CompareTo_SortsByAddressThenNextAddress()
        {
            var p1 = new Position(100, 200);
            var p2 = new Position(200, 300);
            var p3 = new Position(100, 300);

            Assert.True(p1.CompareTo(p2) < 0);
            Assert.True(p2.CompareTo(p1) > 0);
            Assert.True(p1.CompareTo(p3) < 0);
            Assert.Equal(0, p1.CompareTo(new Position(100, 200)));
        }

        [Fact]
        public void IAddressRange_StartAndEnd_MapsCorrectly()
        {
            IAddressRange range = new Position(100, 200);
            Assert.Equal(100, range.Start);
            Assert.Equal(200, range.End);
        }

        [Fact]
        public void ToString_ReturnsFormattedRange()
        {
            var pos = new Position(100, 200);
            Assert.Equal("[100, 200)", pos.ToString());
        }
    }
}
