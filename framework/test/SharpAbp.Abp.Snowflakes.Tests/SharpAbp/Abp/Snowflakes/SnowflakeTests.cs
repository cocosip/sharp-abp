using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;

namespace SharpAbp.Abp.Snowflakes
{
    public class SnowflakeTests
    {
        [Fact]
        public void Should_Generate_Unique_Ids()
        {
            var snowflake = new Snowflake(1, 1);
            var ids = new HashSet<long>();
            for (int i = 0; i < 10000; i++)
            {
                Assert.True(ids.Add(snowflake.NextId()));
            }
        }

        [Fact]
        public void Should_Generate_Increasing_Ids()
        {
            var snowflake = new Snowflake(2, 2);
            long lastId = 0;
            for (int i = 0; i < 10000; i++)
            {
                long currentId = snowflake.NextId();
                Assert.True(currentId > lastId);
                lastId = currentId;
            }
        }

        [Fact]
        public void Should_Throw_Exception_When_Clock_Moves_Backwards()
        {
            var snowflake = new TestableSnowflake(0, 0); // Use a testable version to control time
            snowflake.SetCurrentTime(1000); // Set initial time
            snowflake.NextId(); // Generate first ID

            snowflake.SetCurrentTime(999); // Move clock backwards
            Assert.Throws<InvalidOperationException>(() => snowflake.NextId());
        }

        [Fact]
        public void Should_Handle_Different_Worker_And_Datacenter_Ids()
        {
            var snowflake1 = new Snowflake(1, 1);
            var snowflake2 = new Snowflake(2, 2);

            long id1 = snowflake1.NextId();
            long id2 = snowflake2.NextId();

            Assert.NotEqual(id1, id2);
        }

        [Fact]
        public void Should_Generate_Id_With_Default_Instance()
        {
            var snowflake = Snowflake.Default;
            Assert.True(snowflake.NextId() > 0);
        }

        [Fact]
        public void Should_Throw_ArgumentException_For_Invalid_WorkerId()
        {
            Assert.Throws<ArgumentException>(() => new Snowflake(32, 0)); // Max workerId is 31
            Assert.Throws<ArgumentException>(() => new Snowflake(-1, 0));
        }

        [Fact]
        public void Should_Throw_ArgumentException_For_Invalid_DatacenterId()
        {
            Assert.Throws<ArgumentException>(() => new Snowflake(0, 32)); // Max datacenterId is 31
            Assert.Throws<ArgumentException>(() => new Snowflake(0, -1));
        }

        [Fact]
        public void SnowflakeFactory_GetDefault_ShouldReturnDefaultInstance()
        {
            var factory = new DefaultSnowflakeFactory();
            var defaultSnowflake = factory.GetDefault();
            Assert.Same(Snowflake.Default, defaultSnowflake);
        }

        [Fact]
        public void SnowflakeFactory_Get_ShouldReturnNamedInstance()
        {
            var factory = new DefaultSnowflakeFactory();
            var snowflake1 = factory.Get("test1");
            var snowflake2 = factory.Get("test2");

            Assert.NotNull(snowflake1);
            Assert.NotNull(snowflake2);
            Assert.NotSame(snowflake1, snowflake2);

            var snowflake1Again = factory.Get("test1");
            Assert.Same(snowflake1, snowflake1Again);
        }

        // A testable version of Snowflake to control GetCurrentTimestampMillis for clock rollback tests
        private class TestableSnowflake : Snowflake
        {
            private long _currentTime;

            public TestableSnowflake(long workerId, long datacenterId) : base(workerId, datacenterId)
            {
                _currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

            public void SetCurrentTime(long time)
            {
                _currentTime = time;
            }

            protected override long GetCurrentTimestampMillis()
            {
                return _currentTime;
            }
        }

        // A simple implementation of ISnowflakeFactory for testing purposes
        private class DefaultSnowflakeFactory : ISnowflakeFactory
        {
            private readonly Dictionary<string, Snowflake> _instances = new Dictionary<string, Snowflake>();

            public Snowflake Get(string name)
            {
                if (!_instances.TryGetValue(name, out var instance))
                {
                    instance = new Snowflake(); // For simplicity, always create new default snowflake for named instances
                    _instances[name] = instance;
                }
                return instance;
            }

            public Snowflake GetDefault()
            {
                return Snowflake.Default;
            }
        }
    }
}
