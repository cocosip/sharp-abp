using SharpAbp.Abp.Binary.Protobuf;
using Xunit;

namespace SharpAbp.Abp.Binary
{
    public class ProtobufSerializerTest : AbpBinaryTestBase
    {
        private readonly IBinarySerializer _binarySerializer;
        public ProtobufSerializerTest()
        {
            _binarySerializer = GetRequiredService<IBinarySerializer>();
        }

        [Fact]
        public void SerializeDeserialize()
        {
            var c1 = new TestClass1()
            {
                Id = 1,
                Name = "zhangsan"
            };

            var b1 = _binarySerializer.Serialize(c1);

            var c11 = _binarySerializer.Deserialize<TestClass1>(b1);
            Assert.Equal(1, c11.Id);
            Assert.Equal("zhangsan", c11.Name);
        }

    }
}
