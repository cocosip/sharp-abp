using SharpAbp.Abp.FileStoring.TestObjects;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class FileContainerNameAttributeTest
    {

        [Fact]
        public void Should_Get_Specified_Name()
        {
            var name = FileContainerNameAttribute
                  .GetContainerName<TestContainer2>();

            Assert.Equal("Test2", name);
        }

        [Fact]
        public void Should_Get_Full_Class_Name_If_Not_Specified()
        {
            var expected = typeof(TestContainer1).FullName;

            var name = FileContainerNameAttribute
                  .GetContainerName<TestContainer1>();
            Assert.Equal(expected, name);
        }


        [Fact]
        public void GetName_By_Type()
        {
            var expected = typeof(TestContainer3).FullName;
            var name = FileContainerNameAttribute.GetContainerName(typeof(TestContainer3));
            Assert.Equal(expected, name);
        }

    }
}
