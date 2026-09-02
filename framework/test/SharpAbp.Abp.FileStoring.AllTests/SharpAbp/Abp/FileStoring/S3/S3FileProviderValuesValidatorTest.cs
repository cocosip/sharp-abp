using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.FileStoring.S3
{
    public class S3FileProviderValuesValidatorTest : AbpFileStoringAllTestBase
    {
        [Fact]
        public void Validate_Should_Allow_Empty_AuthenticationRegion()
        {
            var validator = GetRequiredService<IServiceProvider>()
                .GetRequiredKeyedService<IFileProviderValuesValidator>(S3FileProviderConfigurationNames.ProviderName);
            var values = new List<NameValue>
            {
                new(S3FileProviderConfigurationNames.BucketName, "bucket"),
                new(S3FileProviderConfigurationNames.ServerUrl, "http://127.0.0.1:9000"),
                new(S3FileProviderConfigurationNames.AccessKeyId, "AccessKeyId"),
                new(S3FileProviderConfigurationNames.SecretAccessKey, "SecretAccessKey"),
                new(S3FileProviderConfigurationNames.ForcePathStyle, "true"),
                new(S3FileProviderConfigurationNames.UseChunkEncoding, "false"),
                new(S3FileProviderConfigurationNames.Protocol, "0"),
                new(S3FileProviderConfigurationNames.AuthenticationRegion, ""),
                new(S3FileProviderConfigurationNames.CreateBucketIfNotExists, "false")
            };

            var result = validator.Validate(values);

            Assert.Empty(result.Errors);
        }
    }
}
