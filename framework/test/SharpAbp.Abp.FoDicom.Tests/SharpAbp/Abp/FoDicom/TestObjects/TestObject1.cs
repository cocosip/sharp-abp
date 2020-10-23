using FellowOakDicom;

namespace SharpAbp.Abp.FoDicom.TestObjects
{
    public class TestObject1
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DicomTag Tag1 { get; set; }

        public DicomTag Tag2 { get; set; }

        public TestObject1()
        {

        }
    }
}
