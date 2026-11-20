using SharpAbp.Abp.FileStoring.Azure;
using SharpAbp.Abp.FileStoring.FastDFS;
using SharpAbp.Abp.FileStoring.Minio;
using Xunit;

namespace SharpAbp.Abp.FileStoring;

public class CloudStorageNamingTest
{
    [Theory]
    [InlineData("", "000")]
    [InlineData("-A!", "a00")]
    [InlineData("--A---B--", "a-b")]
    public void Azure_And_Minio_Names_Remove_Invalid_Characters_And_Pad_Short_Containers(string input, string expected)
    {
        Assert.Equal(expected, new AzureFileNamingNormalizer().NormalizeContainerName(input));
        Assert.Equal(expected, new MinioFileNamingNormalizer().NormalizeContainerName(input));
    }

    [Fact]
    public void Cloud_Names_Respect_Length_Limits_And_Preserve_Object_Paths()
    {
        foreach (var normalizer in new IFileNamingNormalizer[] { new AzureFileNamingNormalizer(), new MinioFileNamingNormalizer() })
        {
            Assert.Equal(new string('a', 63), normalizer.NormalizeContainerName(new string('A', 70)));
            Assert.Equal("folder/file.bin", normalizer.NormalizeFileName("folder/file.bin"));
        }
        var fast = new FastDFSFileNamingNormalizer();
        Assert.Equal("a000", fast.NormalizeContainerName("-A!"));
        Assert.Equal(new string('a', 16), fast.NormalizeContainerName(new string('A', 20)));
        Assert.Equal("group1/M00/file.bin", fast.NormalizeFileName("group1/M00/file.bin"));
    }
}
