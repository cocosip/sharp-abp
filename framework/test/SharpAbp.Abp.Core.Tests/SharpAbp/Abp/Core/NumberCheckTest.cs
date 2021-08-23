using System;
using Xunit;

namespace SharpAbp.Abp.Core
{
    public class NumberCheckTest
    {
        [Fact]
        public void Positive_Test()
        {
            var n1 = 1;
            var n2 = 0;
            var n3 = -3;
            var n4 = 100L;
            var n5 = -30L;

            Assert.Equal(1, NumberCheck.Positive(n1, "n1"));
            Assert.Throws<ArgumentException>(() =>
            {
                NumberCheck.Positive(n2, "n2");
            });

            Assert.Throws<ArgumentException>(() =>
            {
                NumberCheck.Positive(n3, "n3");
            });

            Assert.Equal(100L, NumberCheck.Positive(n4, "n4"));
            Assert.Throws<ArgumentException>(() =>
            {
                NumberCheck.Positive(n5, "n5");
            });
        }

        [Fact]
        public void Nonnegative_Test()
        {
            var n1 = 32;
            var n2 = 0;
            var n3 = -11;
            var n4 = 60L;
            var n5 = -20L;

            Assert.Equal(32, NumberCheck.Nonnegative(n1, "n1"));
            Assert.Equal(0, NumberCheck.Nonnegative(n2, "n2"));

            Assert.Throws<ArgumentException>(() =>
            {
                NumberCheck.Nonnegative(n3, "n3");
            });
            Assert.Equal(n4, NumberCheck.Nonnegative(n4, "n4"));
            Assert.Throws<ArgumentException>(() =>
            {
                NumberCheck.Nonnegative(n5, "n5");
            });

        }

    }
}
