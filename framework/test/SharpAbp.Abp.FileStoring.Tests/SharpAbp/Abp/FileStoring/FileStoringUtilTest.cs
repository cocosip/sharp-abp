using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class FileStoringUtilTest
    {
        [Fact]
        public void ConvertPrimitiveType_Empty_Test()
        {
            var o1 = FileStoringUtil.ConvertPrimitiveType("", typeof(int), false);
            Assert.Null(o1);

        }

    }
}
