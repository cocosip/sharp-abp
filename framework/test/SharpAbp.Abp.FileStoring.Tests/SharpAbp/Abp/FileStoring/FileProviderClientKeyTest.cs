using System;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderClientKeyTest
    {
        [Fact]
        public void Same_Inputs_Should_Produce_The_Same_Key()
        {
            var first = FileProviderClientKey.Build(
                "Aws",
                ("Region", "us-east-1"),
                ("AccessKeyId", "ak"),
                ("SecretAccessKey", "sk"));

            var second = FileProviderClientKey.Build(
                "Aws",
                ("Region", "us-east-1"),
                ("AccessKeyId", "ak"),
                ("SecretAccessKey", "sk"));

            Assert.Equal(first, second);
        }

        [Fact]
        public void Different_Providers_Should_Produce_Different_Keys()
        {
            var aws = FileProviderClientKey.Build("Aws", ("Region", "us-east-1"));
            var s3 = FileProviderClientKey.Build("S3", ("Region", "us-east-1"));

            Assert.NotEqual(aws, s3);
        }

        [Fact]
        public void Different_Values_Should_Produce_Different_Keys()
        {
            var first = FileProviderClientKey.Build("Aws", ("Region", "us-east-1"));
            var second = FileProviderClientKey.Build("Aws", ("Region", "us-west-2"));

            Assert.NotEqual(first, second);
        }

        [Fact]
        public void Separator_Characters_In_Values_Should_Not_Produce_Collisions()
        {
            var first = FileProviderClientKey.Build(
                "Aws",
                ("A", "x|B=y"),
                ("B", string.Empty));

            var second = FileProviderClientKey.Build(
                "Aws",
                ("A", "x"),
                ("B", "y|B="));

            Assert.NotEqual(first, second);
        }

        [Fact]
        public void Null_And_Empty_Values_Should_Be_Normalized()
        {
            var withNull = FileProviderClientKey.Build("Aws", ("Region", null));
            var withEmpty = FileProviderClientKey.Build("Aws", ("Region", string.Empty));

            Assert.Equal(withNull, withEmpty);
        }
    }
}
