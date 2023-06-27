using ProtoBuf;

namespace SharpAbp.Abp.Binary
{
    [ProtoContract]
    public class TestClass1
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        public TestClass1()
        {

        }
    }
}
